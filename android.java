AppCompatActivity
requestWindowFeature(Window.FEATURE_NO_TITLE);//hide Action Bar

public void onClick(View v) {//toast消息
                Toast.makeText(MainActivity.this, "You clicked Button 1",
                        Toast.LENGTH_SHORT).show();
            }

public void onClick(View v) {//销毁程序
                finish();
            }

public void onClick(View v) {//显式intent切换到另外一个Activity
Intent intent = new Intent(FirstActivity.this, SecondActivity.class);
startActivity(intent);
}

public void onClick(View v) {//隐式调用拨号
Intent intent = new Intent(Intent.ACTION_DIAL);
intent.setData(Uri.parse("tel:10086"));
startActivity(intent);
}

public void onClick(View v) {//向下一个活动传递数据,并打印出来
String data = "Hello SecondActivity";
Intent intent = new Intent(FirstActivity.this, SecondActivity.class);
intent.putExtra("extra_data", data);
startActivity(intent);
}
protected void onCreate(Bundle savedInstanceState) {
super.onCreate(savedInstanceState);
requestWindowFeature(Window.FEATURE_NO_TITLE);
setContentView(R.layout.second_layout);
Intent intent = getIntent();
String data = intent.getStringExtra("extra_data");
Toast.makeText(SecondActivity.this, data,
                Toast.LENGTH_SHORT).show();
}

public void onClick(View v) {//返回数据给上一个活动
Intent intent = new Intent(FirstActivity.this, SecondActivity.class);
startActivityForResult(intent, 1);
}
Button button2 = (Button) findViewById(R.id.button_2);
button2.setOnClickListener(new OnClickListener() {
@Override
public void onClick(View v) {
Intent intent = new Intent();
intent.putExtra("data_return", "Hello FirstActivity");
setResult(RESULT_OK, intent);
finish();
}
});
