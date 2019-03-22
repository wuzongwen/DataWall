layui.use(['layer', 'form'], function () {
    var layer = layui.layer
        , form = layui.form;

    $.ajax({
        type: 'post',
        url: '/Home/GetModuleSet',
        dataType: 'json',
        beforeSend: function () {
        },
        complete: function () {
        },
        error: function (XMLHttpRequest, status, thrownError) {
            console.log('网络繁忙，请稍后重试...');
            return false;
        },
        success: function (data) {
            var code = data.code;
            switch (code) {
                case "200":
                    var ModuleSet = $.parseJSON(data.data.SysSeting);
                    $.each(ModuleSet.Module, function (idx, obj) {
                        if (obj.length > 1) {
                            var i = 0;
                            $('.layui-col-md4').eq(idx).load("/Home/" + obj[0]);
                            setInterval(function () {
                                if (i < obj.length - 1) {
                                    $('.layui-col-md4').eq(idx).load("/Home/" + obj[i + 1]);
                                    i++;
                                }
                                else {
                                    i = 0;
                                    $('.layui-col-md4').eq(idx).load("/Home/" + obj[i]);
                                }
                            }, ModuleSet.Time[idx] * 1000);
                        }
                        else {
                            $('.layui-col-md4').eq(idx).load("/Home/" + obj);
                        }
                    });
                    break;
                case "202":
                    layer.msg('场馆未设置主题', { time: 5000, icon: 5 });
                    break;
            }
        }
    })
});