//elasticdump
[
{"_index":".kibana","_type":"visualization","_id":"test1","_score":0,"_source":{"title":"test1","visState":"{\"type\":\"line\",\"params\":{\"shareYAxis\":true,\"addTooltip\":true,\"addLegend\":true,\"showCircles\":true,\"smoothLines\":false,\"interpolate\":\"linear\",\"scale\":\"linear\",\"drawLinesBetweenPoints\":true,\"radiusRatio\":9,\"times\":[],\"addTimeMarker\":false,\"defaultYExtents\":false,\"setYExtents\":false,\"yAxis\":{}},\"aggs\":[{\"id\":\"1\",\"type\":\"count\",\"schema\":\"metric\",\"params\":{}},{\"id\":\"2\",\"type\":\"date_histogram\",\"schema\":\"segment\",\"params\":{\"field\":\"@timestamp\",\"interval\":\"auto\",\"customInterval\":\"2h\",\"min_doc_count\":1,\"extended_bounds\":{}}}],\"listeners\":{}}","description":"","version":1,"kibanaSavedObjectMeta":{"searchSourceJSON":"{\"index\":\"*\",\"query\":{\"query_string\":{\"query\":\"*\",\"analyze_wildcard\":true}},\"filter\":[]}"}}}
]


//export
[
  {
    "_id": "dashboard_test1",
    "_type": "dashboard",
    "_source": {
      "title": "dashboard_test1",
      "hits": 0,
      "description": "",
      "panelsJSON": "[{\"id\":\"pie-chart\",\"type\":\"visualization\",\"size_x\":3,\"size_y\":2,\"col\":1,\"row\":1},{\"id\":\"test1\",\"type\":\"visualization\",\"size_x\":3,\"size_y\":2,\"col\":4,\"row\":1}]",
      "version": 1,
      "timeRestore": false,
      "kibanaSavedObjectMeta": {
        "searchSourceJSON": "{\"filter\":[{\"query\":{\"query_string\":{\"query\":\"*\",\"analyze_wildcard\":true}}}]}"
      }
    }
  }
]

//curl
{
  "took" : 4,
  "timed_out" : false,
  "_shards" : {
    "total" : 1,
    "successful" : 1,
    "failed" : 0
  },
  "hits" : {
    "total" : 1,
    "max_score" : 1.0,
    "hits" : [ {
      "_index" : ".kibana",
      "_type" : "dashboard",
      "_id" : "dashboard_test1",
      "_score" : 1.0,
      "_source":{"title":"dashboard_test1","hits":0,"description":"","panelsJSON":"[\n  {\n    \"id\": \"pie-chart\",\n    \"type\": \"visualization\",\n    \"size_x\": 3,\n    \"size_y\": 2,\n    \"col\": 1,\n    \"row\": 1\n  },\n  {\n    \"id\": \"test1\",\n    \"type\": \"visualization\",\n    \"size_x\": 3,\n    \"size_y\": 2,\n    \"col\": 4,\n    \"row\": 1\n  }\n]","version":1,"timeRestore":false,"kibanaSavedObjectMeta":{"searchSourceJSON":"{\n  \"filter\": [\n    {\n      \"query\": {\n        \"query_string\": {\n          \"query\": \"*\",\n          \"analyze_wildcard\": true\n        }\n      }\n    }\n  ]\n}"}}
    } ]
  }
}

 [ {"_index" : ".kibana","_type" : "dashboard","_id" : "dashboard_test1","_score" : 1.0,"_source":{"title":"dashboard_test1","hits":0,"description":"","panelsJSON":"[\n  {\n    \"id\": \"pie-chart\",\n    \"type\": \"visualization\",\n    \"size_x\": 3,\n    \"size_y\": 2,\n    \"col\": 1,\n    \"row\": 1\n  },\n  {\n    \"id\": \"test1\",\n    \"type\": \"visualization\",\n    \"size_x\": 3,\n    \"size_y\": 2,\n    \"col\": 4,\n    \"row\": 1\n  }\n]","version":1,"timeRestore":false,"kibanaSavedObjectMeta":{"searchSourceJSON":"{\n  \"filter\": [\n    {\n      \"query\": {\n        \"query_string\": {\n          \"query\": \"*\",\n          \"analyze_wildcard\": true\n        }\n      }\n    }\n  ]\n}"}}} ]

