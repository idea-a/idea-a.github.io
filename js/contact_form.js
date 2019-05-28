$(document).ready(function(){
$("contact-submit").click=function (){
	$.ajax({  
         type:"POST",  
         url: "contact.php",  
            data:{  
                Name: $('#name').val(),  
                Email:$('#email').val(),  
                Phone:$('#phone').val() ,
                Message:$('#message').val()
            },  
         dataType:'text',  
         success:function (data) {  
             if(data=200){  
                alert("提交成功啦")
             }  
         },  
         error:function(err){  
                alert("提交失败")
         }  
     });  
}
});