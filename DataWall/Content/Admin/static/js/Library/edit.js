﻿layui.use(['admin', 'form', 'jquery', 'layer'], function () {
    var form = layui.form,
        $ = layui.jquery,
        admin = layui.admin,
        layer = layui.layer;

    //监听提交
    form.on('submit(add)', function (data) {
        //发异步，把数据提交至后台
        $.ajax({
            type: 'post',
            url: '/Library/LibraryEdit',
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

    //遍历select option
    $(document).ready(function () {
        $('#FatherLibraryId').attr("disabled", "disabled");
        //加载下拉框数据
        if ($("#LibraryType").val() !== "0") {
            $('#FatherLibraryId').removeAttr("disabled");
        }
        $("#Type").val($("#LibraryType").val());
        $('#FatherLibraryId').val($("#LibraryId").val());

        $("#FatherLibraryId option").each(function (text) {
            var level = $(this).attr('data-level');
            var textstr = $(this).text();
            //console.log(text);
            if (level > 0) {
                textstr = "├　" + textstr;
                for (var i = 0; i < level; i++) {
                    textstr = "　　" + textstr;　 //js中连续显示多个空格，需要使用全角的空格
                }
            }
            $(this).text(textstr);
        });
        form.render('select'); //刷新select选择框渲染
    });


    //启用&禁用父场馆下拉框
    form.on('select(type-select)', function (data) {
        var value = String(data.value);
        if (value === "0") {
            $('#FatherLibraryId').attr("disabled", "disabled");
            form.render('select');
        }
        else {
            $('#FatherLibraryId').removeAttr("disabled");
            //选中第一项
            if (FatherLibraryId === 0) {
                $("#FatherLibraryId").val($("#FatherLibraryId option:first").val());
            }
            form.render('select');
        }
    });

    //设置分馆
    form.on('select(pid-select)', function (data) {
        var value = String(data.value);
        $("#FatherLibraryId").val(value);
    });
});