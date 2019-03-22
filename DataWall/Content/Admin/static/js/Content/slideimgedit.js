layui.use(['admin', 'jquery', 'layer', 'upload'], function () {
    var $ = layui.jquery,
        admin = layui.admin,
        upload = layui.upload,
        layer = layui.layer;

    //多图片上传
    upload.render({
        elem: '#FileAdd'
        , url: '/Content/UploadForImageEdit?id=' + $("#Id").val()
        , multiple: true
        , before: function (obj) {
            //加载层-风格4
            layer.load(1, {
                content: '图片上传中',
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
            //预读本地文件示例，不支持ie8
            obj.preview(function (index, file, result) {

            });
        }
        , done: function (res) {
            layer.closeAll("loading");
            //上传完毕
            $('#filelist').append('<a title="删除" id="' + res.fileid + '" onclick="member_del(this,' + res.fileid + ')" href="javascript:;"><img style="max-width:100px;margin-right:5px" src="/' + res.src + '" alt="' + res.fileName + '" class="layui-upload-img"></a>');
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
        , number: 5
    });

    window.member_del = function (obj, id) {
        layer.confirm('确认要删除吗？', function (index) {
            //发异步删除数据
            $.ajax({
                type: 'post',
                url: '/Content/DelImage',
                dataType: 'json',
                data: {
                    id: id
                },
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
                            layer.msg('已删除', {
                                icon: 5,
                                time: 1000
                            });
                            $("#" + id).remove();
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
        });
    }
});