﻿layui.use(['laydate', 'form', 'jquery', 'admin'], function () {
    var laydate = layui.laydate,
        $ = layui.jquery,
        form = layui.form,
        admin = layui.admin;

    //加载结束
    $(document).ready(function () {
        parent.layer.closeAll('loading');
    });

    //赋值
    $("select[name=library]").val($("#libraryId").val());
    form.render("select");

    //翻页搜索条件不重置处理
    $(".pagination > li > a").click(function () {
        event.preventDefault();
        var index = $(this).html();
        if (index === '»') {
            index = parseInt($(".pagination > li[class=active] > a").html()) + 1;
        }
        if (index === '«') {
            index = parseInt($(".pagination > li[class=active] > a").html()) - 1;
        }
        if (index < 1) return;
        $("input[name=page]").val(index);
        $("#frmSearch").submit();
    });

    //执行一个laydate实例
    laydate.render({
        elem: '#start' //指定元素
    });
    //执行一个laydate实例
    laydate.render({
        elem: '#end' //指定元素
    });
    /*通知公告-停用*/
    window.member_stop = function (obj, id) {
        var title, enable;
        if ($(obj).attr('title') === '启用') {
            title = "启用";
            enable = 0;
        }
        else {
            title = "停用";
            enable = 1;
        }
        layer.confirm('确认要' + title + '吗？', function (index) {
            //发异步把用户状态进行更改
            $.ajax({
                type: 'post',
                url: '/Content/EditEnable',
                dataType: 'json',
                data: {
                    id: id,
                    enable: enable
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
                            if (enable === 1) {
                                $(obj).attr('title', '启用')
                                $(obj).find('i').html('✔');
                                $(obj).parents("tr").find(".td-status").find('span').addClass('layui-btn-disabled').html('已' + title);
                                layer.msg('已' + title, {
                                    icon: 5,
                                    time: 1000
                                });
                            }
                            else {
                                $(obj).attr('title', '停用')
                                $(obj).find('i').html('✘');
                                $(obj).parents("tr").find(".td-status").find('span').removeClass('layui-btn-disabled').html('已' + title);
                                layer.msg('已' + title, {
                                    icon: 6,
                                    time: 1000
                                });
                            }
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

    /*通知公告-删除*/
    window.member_del = function (obj, id, type, page) {
        layer.confirm('确认要删除吗？', function (index) {
            //发异步删除数据
            $.ajax({
                type: 'post',
                url: '/Content/DelContent',
                dataType: 'json',
                data: {
                    id: id,
                    page: page
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
                            layer.alert(msg.msg, { icon: 1 }, function () {
                                location.href = "/Content/ContentList?page=" + msg.page + "&Type=" + type;
                            });
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

    window.delAll = function (type, page) {
        var data = tableCheck.getData();
        if (data.length > 0) {
            layer.confirm('确认要删除吗？', function (index) {
                //发异步删除数据
                $.ajax({
                    type: 'post',
                    url: '/Content/DelContentAll',
                    dataType: 'json',
                    data: {
                        idList: String(data),
                        page: page
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
                                layer.alert(msg.msg, { icon: 1 }, function () {
                                    location.href = "/Content/ContentList?page=" + msg.page + "&Type=" + type;
                                });
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
        else {
            layer.msg('请选择需要删除的内容', {
                icon: 5,
                time: 1000
            });
        }
    }
});