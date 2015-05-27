

$(function () {

    Lover.init();


    //return Lover;
});

var Lover ={
    options:{

    },

    init:function () {
        alert("sdkj");
        this.getInitData();
    },

    getInitData:function () {
        $.ajax({
            url: "/ajax.ashx",
            data: { action: "getInitData" },
            dataType:'JSON',
            success:function(){

            },
            error: function () {
            }

        })

    }
};
