$(document).ready(function () {
    $("#btnAddCategory").click(function () {
        var newCategory = $("#tbCategoryName").val();
        $("#tbCategoryName").val("");
        if (newCategory.length == 0) {
            alert("Please enter a category name first.");
            return false;
        }
        else { 
            //check for duplication
            var opt = $('#Review_RCategoryId option').filter(function () { return $(this).html() == newCategory; });
            if (opt.val() != undefined) {
                $("#Review_RCategoryId option").removeAttr("selected");
                opt.attr("selected", "selected");
            }
            else {
                //Add Category into dropdown with value = "-2"
                $("#Review_RCategoryId").append("<option value='-2' selected='selected'>" + newCategory + "</option>");
                $("#NewCategoryName").val(newCategory);
                //Hide the add category div
            }
            $("#liAddNewCategory").slideUp();
        }
        
    });

    $("#btnCancelCategory").click(function () {
        $("#liAddNewCategory").slideUp();
    });

    $("#Review_RCategoryId").change(function () {
        if ($(this).find("option").length > 1) {
            $("#btnCancelCategory").removeAttr("class");
            $("#btnCancelCategory").show();
        }
            
        if ($(this).find("option:selected").text() == "-- Add my own --")
            $("#liAddNewCategory").slideDown();
        else
            $("#liAddNewCategory").slideUp();

    });

    $(".rateit").bind("rated", function () {
        $("#Review_Rating").val($(this).rateit("value"));
    });
});