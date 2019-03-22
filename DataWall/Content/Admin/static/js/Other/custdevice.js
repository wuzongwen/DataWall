layui.use(['admin', 'form', 'jquery', 'layer'], function () {
    var form = layui.form,
        $ = layui.jquery,
        admin = layui.admin,
        layer = layui.layer;

    //修改客流设备
    form.on('submit(edit)', function (data) {
        //发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/Other/CustDeviceEdit',
            dataType: 'json',
            data: $('.layui-form').serialize(),
            beforeSend: function () {
                //加载层-风格4
                layer.load(1, {
                    content: '数据提交中',
                    shade: [0.4, '#393D49'],
                    // time: 10 * 1000,
                    success: function (layero) {
                        layero.css('padding-left', '30px');
                        layero.find('.layui-layer-content').css({
                            'padding-top': '40px',
                            'width': '70px',
                            'background-position-x': '16px'
                        });
                    }
                });
            },
            complete: function () {
                layer.closeAll("loading");
            },
            error: function (XMLHttpRequest, status, thrownError) {
                layer.msg('网络繁忙，请稍后重试...');
                return false;
            },
            success: function (msg) {
                var code = msg.code;
                switch (code) {
                    case '200':
                        layer.alert(msg.msg, { icon: 6 }, function () {
                            // 获得frame索引
                            var index = parent.layer.getFrameIndex(window.name);
                            //关闭当前frame
                            parent.layer.close(index);
                            //刷新父页面
                            parent.location.reload();
                        });
                        break;
                    case '201':
                        layer.msg(msg.msg);
                        break;
                    case '202':
                        layer.msg(msg.msg);
                        break;
                    case '401':
                        layer.msg(msg.msg, {
                            icon: 5,
                            time: 1000
                        });
                        break;
                    case '402':
                        layer.alert(msg.msg, { icon: 5 }, function () {
                            //跳转到登录页
                            top.location.href = msg.url;
                        });
                        break;
                }
                return false;
            }
        });
        return false;
    });

    //添加客流设备
    form.on('submit(add)', function (data) {
        //发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/Other/CustDeviceAdd',
            dataType: 'json',
            data: $('.layui-form').serialize(),
            beforeSend: function () {
                //加载层-风格4
                layer.load(1, {
                    content: '数据提交中',
                    shade: [0.4, '#393D49'],
                    // time: 10 * 1000,
                    success: function (layero) {
                        layero.css('padding-left', '30px');
                        layero.find('.layui-layer-content').css({
                            'padding-top': '40px',
                            'width': '70px',
                            'background-position-x': '16px'
                        });
                    }
                });
            },
            complete: function () {
                layer.closeAll("loading");
            },
            error: function (XMLHttpRequest, status, thrownError) {
                layer.msg('网络繁忙，请稍后重试...');
                return false;
            },
            success: function (msg) {
                var code = msg.code;
                switch (code) {
                    case '200':
                        layer.alert(msg.msg, { icon: 6 }, function () {
                            // 获得frame索引
                            var index = parent.layer.getFrameIndex(window.name);
                            //关闭当前frame
                            parent.layer.close(index);
                            //刷新父页面
                            parent.location.reload();
                        });
                        break;
                    case '201':
                        layer.msg(msg.msg);
                        break;
                    case '202':
                        layer.msg(msg.msg);
                        break;
                    case '401':
                        layer.msg(msg.msg, {
                            icon: 5,
                            time: 1000
                        });
                        break;
                    case '402':
                        layer.alert(msg.msg, { icon: 5 }, function () {
                            //跳转到登录页
                            top.location.href = msg.url;
                        });
                        break;
                }
                return false;
            }
        });
        return false;
    });

    //select赋值
    $(document).ready(function () {
        $('#Library').val($('#LibraryId').val());
        form.render('select'); //刷新select选择框渲染
    });
});