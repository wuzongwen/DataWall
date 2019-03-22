layui.use(['laydate', 'form', 'jquery', 'admin'], function () {
    var laydate = layui.laydate,
        $ = layui.jquery,
        form = layui.form,
        admin = layui.admin;

    //加载结束
    $(document).ready(function () {
        parent.layer.closeAll('loading');
    });

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
});