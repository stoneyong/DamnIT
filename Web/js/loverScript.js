

$(function () {
    
    var Lover = {
        options:{

        },

        init:function(){
            this.getInitData();
        },

        getInitData: function () {
            $.ajax({
                url: "",
                data:{action:"getUserData"},
                dataType:'JSON',
                success:function(){

                },
                error: function () {
                }

            })

        },
};
    return Lover;
});
