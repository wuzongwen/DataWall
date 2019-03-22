layui.use(['laydate','admin', 'form', 'jquery', 'layer', 'upload'], function () {
    var form = layui.form,
        $ = layui.jquery,
        laydate = layui.laydate,
        admin = layui.admin,
        upload = layui.upload,
        layer = layui.layer;

    //上传
    var uploadInst = upload.render({
        elem: '#UploadFile' //绑定元素
        , url: '/Content/Upload' //上传接口
        , before: function (obj) {
            //加载层-风格4
            layer.msg('封面上传中', {
                icon: 16
                , shade: 0.01
                , time: 60 * 1000
            });
        }
        , done: function (res, index, upload) {
            //上传完毕回调
            if (res.src !== "" && res.src !== null) {
                $(".bookimg img").remove();
                $('.bookimg').append('<img src="' + res.src + '" class="" width="115" height="172" >');
                $('input[name=BookImg]').val(res.src);
                //关闭
                layer.closeAll("loading");
                layer.msg("上传成功");
            }
        }
        , error: function () {
            //请求异常回调
            layer.msg('网络繁忙，请稍后重试...');
            layer.closeAll("loading");
            return false;
            //关闭
        }
        , accept: 'images'
        , size: 10240
    });

    //自定义验证规则
    form.verify({
        BookImg: function (value) {
            if (value.length === 0) {
                return '请上传图书封面';
            }
        }
    });

    //获取书籍信息
    window.getBookinfo = function () {
        var ISBN = $('#ISBN').val();
        if (ISBN.length !== 0) {
            //发异步删除数据
            $.ajax({
                type: 'post',
                url: '/Content/GetBookInfo',
                dataType: 'json',
                data: {
                    ISBN: ISBN
                },
                beforeSend: function () {
                    //加载层-风格4
                    layer.msg('信息获取中', {
                        icon: 16
                        , shade: 0.01
                        , time: 60 * 1000
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
                            var bookinfo = JSON.parse(msg.bookinfo);
                            //设置书籍信息
                            $("input[name=BookName]").val(bookinfo.title);
                            $("input[name=Author]").val(bookinfo.author);
                            $("input[name=Press]").val(bookinfo.publisher);
                            $("input[name=PublishDate]").val(bookinfo.pubdate);
                            $("textarea[name=BookDescribe]").val(bookinfo.summary);
                            $(".bookimg img").remove();
                            $('.bookimg').append('<img src="' + msg.bookimg + '" class="" width="115" height="172" alt="' + bookinfo.title + '">');
                            $('input[name=BookImg]').val(msg.bookimg);
                            layer.msg("获取成功");
                            break;
                        case '201':
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
        }
        else {
            layer.msg("请填写书籍ISBN");
        }
    }

    //添加新书推荐
    form.on('submit(add)', function (data) {
        //发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/Content/NewBookAdd',
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

    //修改新书推荐
    form.on('submit(edit)', function (data) {
        //发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/Content/NewBookEdit',
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