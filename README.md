# myCode_copy
# Description:
#   Interact with Yammer
#
# Dependencies:
#   "request": "~2"
#
# Configuration:
#

request = require 'request'

yammer_base_url = 'https://api.yammer.com/api/v1/'
yammer_oauth2_token_url = "https://www.yammer.com/oauth2/access_token"

oauth2_getToken = (robot, client_id, client_secret, code, cb) =>
  url = yammer_oauth2_token_url
  data = "client_id=#{client_id}&client_secret=#{client_secret}&code=#{code}&grant_type=authorization_code"
  options =
    method: 'POST'
    url: url
    #rejectUnauthorized: false
    body: data

  message = "\n\nurl is #{url}\n"
  message = message + "data is #{data}\n"
  message = message + "options are #{JSON.stringify(options,null,2)}\n"

  robot.logger.debug message
  httpRequest options, cb

listUserGroups = (token, callback) =>
  path = 'groups.json?mine=1'
  httpGet token, path, (error, result) =>
    return callback error if error?
    groups = ({id: g.id, name: g.name, full_name: g.full_name} for g in result)
    callback null, groups

postMessage = (token, group, message, callback) =>
  path = 'messages.json'
  listUserGroups token, (error, groups) =>
    return callback error if error?
    groups =(g for g in groups when g.name.toLowerCase() == group.toLowerCase())
    if groups.length == 0
      return callback(new Error "Not found group #{group}")

    data = 
      body: message
      group_id: groups[0].id
    httpPost token, path, data, (error, result) =>
      return callback error if error?
      callback null, result

httpGet = (token, path, callback) =>
  url = "#{yammer_base_url}#{path}"
  options =
    method: 'GET'
    url: url
    rejectUnauthorized: false
    auth:
      bearer: token
    json: true

  httpRequest options, callback

httpPost = (token, path, data, callback) =>
  url = "#{yammer_base_url}#{path}"
  options =
    method: 'POST'
    url: url
    rejectUnauthorized: false
    auth:
      bearer: token
    json: true
    body: data

  httpRequest options, callback

httpRequest = (options, callback) =>
  request options, (err, res, body) ->
    if err?
      return callback err
    if res.statusCode >= 300
      err = new Error res.statusCode
      err.body = body
      return callback err

    callback null, body

exports.postMessage = postMessage
exports.listUserGroups = listUserGroups
exports.oauth2_getToken = oauth2_getToken




# Description:
#   Interact with Yammer
#
# Dependencies:
#   "./utilities/yammer"
#
# Configuration:
#   YAMMER_APP_NAME - Name of yammer app that hammer will use to post on user's behalf
#   YAMMER_CLIENT_ID - 'Client ID' tied to app posting on user's behalf. Register a new app here https://www.yammer.com/client_applications
#   YAMMER_CLIENT_SECRET - 'Client secret' tied to app posting on user's behalf. KEEP THIS SECRET.
#   YAMMER_HUBOT_REDIRECT_URI - 'Expected redirect' tied to app posting on user's behalf. Set it to the hostname or IP of server hosting your bot. Example http://c0VR9000.itcs.hp.com
#
# Commands:
#   hubot yammer forget me - remove the flow user's yammer access token from hubot
#   hubot yammer list users - list flow users whose yammer access tokens are remembered
#   hubot yammer post in <group> <message> - post message to yammer group. The flow user must be a member of the specified group.
#   hubot yammer auth me - initiate oauth2 flow to get user's yammer oauth2 access token and save in hubot so that hubot can post messages to yammer on behalf of user via "Hammer-Yammer" app
#

# DEPS
querystring = require('querystring')
request = require 'request'
yammer = require './utilities/yammer'
util = require 'util'

# ENV VARS
yammer_app_name = process.env.YAMMER_APP_NAME
client_id = process.env.YAMMER_CLIENT_ID
client_secret = process.env.YAMMER_CLIENT_SECRET
hubot_redirect_url = process.env.YAMMER_HUBOT_REDIRECT_URI
hubot_name= process.env.HUBOT_NAME

# GLOBALS
oauth2_auth_url_base = "https://www.yammer.com/oauth2/authorize?client_id=#{client_id}&redirect_uri="
debug = false

module.exports = (robot) ->
  unless robot.brain.data.yammer?
    robot.brain.data.yammer = {}
  unless robot.brain.data.yammer_req_sessions?
    robot.brain.data.yammer_req_sessions = {}

  robot.respond /yammer test$/i, (msg) ->
    user = msg.message.user
    robot.logger.debug util.inspect(msg) 

  robot.respond /(?:yammer) list users/i, (msg) ->
    users = robot.brain.data.yammer

    response = ""
    for username of users
      response = response + "#{username}\n"
    response.trim()
    msg.send response

  robot.respond /(?:yammer) forget me/i, (msg) ->
    username = msg.message.user.name
    users = robot.brain.data.yammer

    delete users[username]
    msg.send "Yammer has forgotten #{username}"

  #robot.respond /(?:yammer) post in (\S+)\s+(\s\S)/i, (msg) ->
  robot.respond /(?:yammer) post in (\S+)\s+((.|\n)*)/i, (msg) ->
    group = msg.match[1]
    message = msg.match[2]
    username = msg.message.user.name
    users = robot.brain.data.yammer

    if username of users
      token = users[username]
      yammer.postMessage token, group, message, (error, result) =>
        if error?
          robot.logger.error "Failed to post message to yammer: ", error
          return  msg.send "Failed to post message to yammer: #{error}"
        msg.send "The message has been posted to yammer."
    else
      response = "Yammer doesn't know you.\n"
      response = response + "Let yammer remember you: `yammer remember me <token>`"
      msg.send response

  robot.respond /(?:yammer) auth me/i, (msg) ->
    room = msg.message.room
    username = msg.message.user.name
    session = msg.message.id
    request_sessions = robot.brain.data.yammer_req_sessions

    request_sessions[username] = session
    auth_url = oauth2_auth_url_base + hubot_redirect_url + "/hubot/yammer/#{room}/?state=#{username}_#{session}"
    message = "Click the following link to authorize #{hubot_name} to post on your behalf  using yammer app #{yammer_app_name}\n"
    message = message + auth_url + "\n"
    msg.send message

  robot.router.get '/hubot/yammer/:room/', (req, res) ->
    room = req.params.room
    query = querystring.parse(req._parsedUrl.query)
    code = query.code
    state = query.state
    users = robot.brain.data.yammer
    request_sessions = robot.brain.data.yammer_req_sessions

    [user , session] = state.split('_')

    if(debug)
      robot.logger.debug "room is #{room}\ncode is #{code}\nstate is #{state}\nuser is #{user}\nsession is #{session}\n"

    expected_session = request_sessions[user]

    #if !user or !session or !request_sessions[user]
    if !user or !session
      err_msg = "Missing session defailts. Please request yammer oauth2 flow again"
      robot.logger.error err_msg
      robot.logger.debug "user is #{user}\nsession is #{session}"
      robot.messageRoom room, err_msg
      return res.end err_msg
    else if !request_sessions[user]
      err_msg = "invalid oauth2 session, session may be hijacked. Please request yammer oauth2 flow again"
      robot.logger.error err_msg
      robot.logger.debug "user is #{user}\nsession is #{session}. no saved session on requestion present, user may be faking as someone else"
      #robot.messageRoom room, err_msg
      return res.end err_msg
    else if expected_session is not session
      err_msg = "invalid oauth2 session. Please request yammer oauth2 flow again"
      robot.logger.error err_msg
      robot.logger.debug "session is #{session}\nexpected_session is #{expected_session}"
      #robot.messageRoom room, err_msg
      return res.end err_msg

    if(debug)
      message = "room is #{room}\ncode is #{code}\nuser is #{user}\nsession is #{session}. requesting oauth2 token from yammer"
      robot.messageRoom room, message

    yammer.oauth2_getToken robot, client_id, client_secret, code, (err, body) =>
      if err?
        err_msg = "Error getting oauth2 token from yammer: #{err.message}"
        robot.logger.error err_msg, err
        robot.messageRoom room, err_msg
        return res.end "Error getting oauth2 token from yammer: #{err.message}"
      
      data = null
      try
        data = JSON.parse(body)
      catch error
        return res.end "Ran into an error parsing JSON"

      oauth2_token = data.access_token.token
      user_fullName = data.user.full_name

      users[user] = oauth2_token
      delete request_sessions[user]

      if(debug)
        robot.logger.debug "yammer body response is \n#{JSON.stringify(body,null,2)}"
        robot.logger.debug "user_fullName is #{user_fullName} token is #{oauth2_token}"
 
      message = "Got your auth code!\n\n"
      message = message + "Yammer user requesting auth is : #{user_fullName}\n"
      message = message + "Yammer oauth2 token for user #{user} has been saved and yammer app #{yammer_app_name} can now post messages to yammer groups on your behalf\n"
      message = message + "To post do the following: #{hubot_name} yammer post in <YAMMER_GROUP> <MESSAGE>"
      robot.messageRoom room, message
      res.end message
