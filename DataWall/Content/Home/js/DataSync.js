$(function () {
    //声明hub代理
    var chat = $.connection.myHub;
    //创建后端要调用的前端function
    chat.client.sendAllMessage = function (message) {
        console.log("服务端群发消息:" + message);
    };
    //对应后端的SendMessage函数，消息接收函数
    chat.client.sendMessage = function (message) {
        //更新类型 type 0:照片墙 1:风采展示 2:通知公告 3:客流 4:借还 5:新书推荐 6:主题更新
        var Json = JSON.parse(message);
        switch (Json.type) {
            case 3:
                $('#kltj').html("客流统计更新了");
                break;
        }
        //console.log("类型:" + Json.type + ";" + Json.msg);
    };
    chat.client.hello = function (message) {
        $('#discussion').append('<li><strong>服务器广播的消息：hello</strong></li>');
    };
    chat.client.isLogin = function (name, message) {
        console.log(name + message);;
    };
    $.connection.hub.start().done(function () {
        //发送上线信息
        chat.server.sendLogin(LibraryName);
    });
});