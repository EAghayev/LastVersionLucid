
$(document).ready(function () {
 
    if ($("#toast-container").length > 0) {
        $("#toast-container").fadeOut(2500);
    }


    if ($(".report-main").length > 0) {
        $('[data-toggle="tooltip2"]').tooltip();
    }
 
    //navigation active
    var current = location.pathname.split("/");
    $('.sidebar-main-menu li').each(function () {
        $(this).find(".active").removeClass("active")
    })
    $('.sidebar-main-menu li a').each(function () {
        var $this = $(this);

        // if the current path is like this link, make it active
        if (current[1] == "" || current[1] == "home" || current[1] == "Home") {
            if ($this.attr('href') == "/") {
                $this.addClass('active')
            }
        }
        else if ($this.attr('href').includes(current[1])) {
            if ($this.parents().eq(1).prev().hasClass("sidebar-dropdown-toggle")) {
                $this.parents().eq(1).prev().addClass("active")
                return
            }
            else {
                $this.addClass('active');
            }

        }
    })
    if (current[1] == "") {
        current[1]="Dashboard"
    }
    $(".navigation-link-main").text(current[1])

    //navigation breadcrumbs
    $('.main-content-header').find('.navigation-link').remove('.navigation-link')
    for (var i = 1; i < current.length; i++) {
        if (current[i] == "") {
          current[i]="Dashboard"
        }
        var breadcrumbLink = `<li class="navigation-link"><span>` + current[i] + `</span></li>`
        $('.main-content-header').find('.breadcrumbs').append(breadcrumbLink)
    }


   //dt-picker
    $('#datetimepicker1').datetimepicker({
        autoclose: true,
        startDate: new Date(),
        widgetPositioning: {
            horizontal: 'auto',
            vertical: 'bottom'
        }

    });
    //}
$(".mynav-toggler-btn").click(function(e){
    if($(".bell-toggle").is(e.target) || $(".icon-bell").is(e.target)){
        $(".equalizer-menu").css("display","none")
    }
    else{
        $(".bell-menu").css("display","none");
    }
$(this).next().toggle()
$(this).next().css("animation-name","mynav-dropdown")
})

$(".sidebar-account-toggler").click(function(e){
    $(this).next().toggle();
    $(this).next().css("animation-name","sidebar-account")
})
$(window).click(function(e){
  
       if(!$(".mynav-toggler-btn").is(e.target) && $(".mynav-toggler-btn").has(e.target).length===0
            && !$(".bell-menu").is(e.target) && $(".bell-menu").has(e.target).length===0 
            && !$(".equalizer-menu").is(e.target) && $(".equalizer-menu").has(e.target).length===0){
           $(".bell-menu").hide();
           $(".equalizer-menu").hide();
       }
       if(!$(".sidebar-account-toggler").is(e.target) && $(".sidebar-account-toggler").has(e.target).length===0){
           $(".account-dropdown-menu").hide()
       }
       if($(".modal").length){
           if($(".modal").is(e.target)){
            $(".modal").css("display","none")
            $(".modal-overlay").css("display","none")
            $("body").css("overflow","auto")
           }
        }
       

    })

    $(".menu-bar li").click(function(){
        $(".menu-bar li").removeClass("active")
        $(this).addClass("active")
    })
    // $(".sidebar-dropdown-toggle").click(function(){
        
    // })
    $(".sidebar-dropdown-toggle").click(function(){
      
        if( $(this).find(".sidemenu-dropdown").hasClass("dropdown-animate-icon")){
            $(this).find(".sidemenu-dropdown").addClass("dropdown-icon-animate")
            $(this).find(".sidemenu-dropdown").removeClass("dropdown-animate-icon")
            $(this).next().slideUp("slow")
            return false;
      
        }
        else
        if( $(this).find(".sidemenu-dropdown").hasClass("dropdown-icon-animate")){
            $(this).find(".sidemenu-dropdown").addClass("dropdown-animate-icon")
            $(document).find(".sidemenu-dropdown").removeClass("dropdown-icon-animate")
            $(".sidebar-dropdown-menu").slideUp("slow");
            $(this).next().slideDown("slow")
            return false;  
        }
        else{
            $(document).find(".sidemenu-dropdown").removeClass("dropdown-animate-icon")
            $(this).find(".sidemenu-dropdown").addClass("dropdown-animate-icon")
             $(".sidebar-dropdown-menu").slideUp("slow");
             $(this).next().slideToggle("slow")
            return false;
        }
    
        return false;
    })
    
    $(".navbar-toggle").click(function(){
        if($("#sidebar").css("left")=="-250px"){
            $("#sidebar").css("left","5px")
        }
        else {
            $("#sidebar").css("left","-250px")
        }
    })
    $(".sidebar-arrow").click(function(){
        
        if($(window).width()>1200){
            if($("#sidebar").css("left")=="-250px"){
                $("#main-content").css("width","calc(100% - 250px)")
                $("#sidebar").css("left","5px")
                $(".sidebar-arrow").addClass("fa-arrow-left")
                $(".sidebar-arrow").removeClass("fa-arrow-right")
            }
            else {
                $("#main-content").css("width","100%")  
                $("#sidebar").css("left","-250px")
                $(".sidebar-arrow").addClass("fa-arrow-right")
                $(".sidebar-arrow").removeClass("fa-arrow-left")
            }
        }
    })
    $(window).resize(function(){

        if($(this).width()>1200){
            $("#sidebar").css("left","5px")
            $("#main-content").css("width","calc(100% - 250px)")
        }
        else{
            $("#sidebar").css("left","-250px")
            $("#main-content").css("width","100%")
        }
    })
   
    //==================================================================
    //To-Do-List
    //================================================================
    $(document).on("click", ".to-do-checkbox input", function () {
        var state = $(this).next().find("label").text();
        var id = $(this).val();
        var isCompleted;
        if ($(this).parent().find("input").prop("checked") == true) {
            $(this).next().find("label").addClass("line-through")
            isCompleted = true;
        }
        else {
            $(this).next().find("label").removeClass("line-through")
            isCompleted = false;
        }
        $.ajax({
            url: "/home/completed?isCompleted=" + isCompleted + "&id=" + id+"&",
            dataType: "json",
            type: "post",
            success: function (response) {

            }
        })
    })
  
   
    //==================================================================
    //Check all employee 
    //================================================================
    $("#employee-checkbox-all").click(function(){
        if($("#employee-checkbox-all").prop("checked")==true){
            $("tbody input").prop("checked",true);
        }
        else{
            $("tbody input").prop("checked",false); 
        }
    })
    
     //===================================================================
    //Calendar
    //===================================================================
    if ($("#calendar").length) {
      
        $.getJSON("/events/calendar", function (response) {
            if (status = 200) {
                var events = [];
                var currentdate = new Date();
                var defaultDate;
                class myEvent {
                    constructor(title, start, end, className) {
                        this.title = title,
                            this.start = start,
                            this.end = end,
                            this.className = className
                    }
                }
                var className;
                defaultDate = response.dateNow
                $.each(response.data.evnt, function (key, value) {
                    var date = currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getMilliseconds();
                  
                    if (value.Type == "Optional") {
                        className = "bg-dark"
                    }
                    else if (value.Type == "Important") {
                        className = "bg-danger"
                    }
                    else {
                        className = "bg-primary"
                    }
                    var event = new myEvent(value.Title, value.StartDate + "T" + value.StartTime, value.EndDate+"T"+value.EndTime, className)
                    events.push(event);
                })

                $('#calendar').fullCalendar({
                    //header: {
                    //    left: 'prev,next today',
                    //    center: 'title',
                    //    right: 'month,agendaWeek,agendaDay,listWeek'
                    //}
                    header: {
                        left: 'prev',
                        center: 'title',
                        right: 'next'
                    },
                    defaultDate: defaultDate,
                    editable: true,
                    droppable: true, // this allows things to be dropped onto the calendar
                    eventDrop: function (event) {
                        var start = event.start._i;
                        var end = event.end._i;
                        var data = [start, end];
                        $.ajax({
                            url: "/events/dropevent",
                            dataType: "json",
                            type:"post",
                            data: data,
                            success: function (response) {
                                $('#calendar').empty();
                                loadCalendar();
                            }
                        })
                    },
                    eventResize: function (event, delta, revertFunc) {

                    },
                    drop: function () {
                        // is the "remove after drop" checkbox checked?
                        if ($('#drop-remove').is(':checked')) {
                            // if so, remove the element from the "Draggable Events" list
                            $(this).remove();
                        }
                    },
                    eventLimit: true, // allow "more" link when too many events
                    events: events
                })

            }
        })

}
    
    //===================================================================
    //DataTables
    //===================================================================
      if($("#data-table").length){
        if($('.table')){
            $('.table').DataTable();
            $("#DataTables_Table_0_wrapper").find("select").removeClass("custom-select");
        }
      }
       
       

        //===================================
        //progress-bar
        //====================================
    if ($("#circle-first").length) {
        $.getJSON("/home/Progressbar", function (response) {
            if (response.status == 200) {
                $("#circle-first").circleProgress({
                    progress: 0.75,
                    thickness: 8,
                    value: response.data.male/100,
                    percentage: true,
                    lineCap: 'round',
                    size: 95,
                    fill: {
                        gradient: ["rgb(73, 169, 229)"]
                    }
                }).on('circle-animation-progress', function (event, progress, stepValue) {
                    var myval = response.data.male
                    $(this).find('strong').text(myval + "%");
                    })

                $("#circle-second").circleProgress({
                    progress: 0.75,
                    value: response.data.female/100,
                    thickness: 8,
                    percentage: true,
                    lineCap: 'round',
                    size: 95,
                    fill: {
                        gradient: ["rgb(184, 128, 225)"]
                    }
                }).on('circle-animation-progress', function (event, progress, stepValue) {
                    var myval = response.data.female
                    $(this).find('strong').text(myval + "%");
                })
            }
        });
    }

//=============================================================================
//Add new in modal
//=============================================================================
    $("#modal-add-new").click(function () {
        $("#add-emp")[0].reset()
        $(".add-uploaded")[0].reset()
        $(".edit-emp-btn").hide();
        $(".add-emp-btn").show();
        //remove profile
        $(".delete-uploaded").remove();
        $(".img img").remove();
        $("#upload").prop("disabled", false)
        $("#upload-lbl").css("cursor", "pointer")

    $(".modal").css("display","block")
    $(".modal-overlay").css("display","block")
    $("body").css("overflow","hidden")
})

$(".close-modal").click(function(e){
    e.preventDefault()
    $(".modal").css("display","none")
    $(".modal-overlay").css("display","none")
    $("body").css("overflow","auto")
})
//=============================================================================
//Attendance List icons
//=============================================================================

if($("#attendance-list").length){
    $("#attendance-list .attn-day").click(function(){
        if($(this).find("i").hasClass("icon-close")){
            $(this).find("i").removeClass("icon-close").addClass("icon-check")
        }
        else{
            $(this).find("i").removeClass("icon-check").addClass("icon-close")
        }
    })
}

//=============================================================================
//Select gender radio button in employee form
//=============================================================================
    $(".gender").click(function () {
        var myinput = $(this).find("input");

    myinput.prop("checked") == true ? myinput.prop("checked", false) : myinput.prop("checked", true)
        if ($(".fa-male").next().prop("checked") == true) {
            myinput.parent().prevAll("span").text("Male")
        }
        else if ($(".fa-female").next().prop("checked") == true) {
            myinput.parent().prevAll("span").text("Female")
        }
        else {
            myinput.parent().prevAll("span").text("Gender")
        }
    })

    //
    $(".mystacked-buttons li a").click(function () {
        $(".mystacked-buttons").find(".active").removeClass("active");
        $(this).addClass("active")
    })



    //Leave Requests

    $(".request-reject").click(function (e) {
        var url = $(this).attr("href")
        e.preventDefault(); 
        swal({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.value) {
                window.location.replace(url)
               
            }
        })
    })

    //Add Leave
    $("#modal-add-leave").click(function () {
        $("#leave-form")[0].reset();

        $(".modal").css("display", "block")
        $(".modal-overlay").css("display", "block")
        $("body").css("overflow", "hidden")
    })
    //open holiday modal
    $(".open-modal-holiday").click(function () {
        $(".modal").css("display", "block")
        $(".modal-overlay").css("display", "block")
        $("body").css("overflow", "hidden")
        $(".modal-add-buttons .edit-hol-btn").hide();
        $(".modal-add-buttons .add-hol-btn").show();    
        $(".holiday-form").attr("action", "/holidays/create")
        $(".holiday-form")[0].reset();
    })

    $('input[name="LeaveTypeId"]').change(function () {
    })

    ////leave-form submit inputs controls
    //    var errorMessage1 = "EmployeeId is not correct";
    //    var errorMessage2 = "Leave type is not correct";
    ////EmployeeId control
    //$('input[name="EmployeeId"]').focusout(function () {
    //    if (!$('input[name="EmployeeId"]').val().includes('-')) {
    //        if ($(".toast-error").length > 0 && $(".toast-error .toast-message").text() == errorMessage1) {
    //            $(".toast-error").remove()
    //        }
    //        toastr.error(errorMessage1)
    //    }
    //    else {
    //        var empIdArr = $('input[name="EmployeeId"]').val().split('-')
    //        if (empIdArr.length != 2) {
    //            if ($(".toast-error").length > 0 && $(".toast-error .toast-message").text() == errorMessage1) {
    //                $(".toast-error").remove()
    //            }
    //            toastr.error(errorMessage1)
    //        }
    //        if (isNaN(empIdArr[1]) || empIdArr[0] != "LA") {
    //            if ($(".toast-error").length > 0 && $(".toast-error .toast-message").text() == errorMessage1) {
    //                $(".toast-error").remove()
    //            }
    //            toastr.error(errorMessage1)
    //        }
    //    }
    //})
        
            

    //Leavetype control
    $('input[name="LeaveTypeId"]').focusout(function () {
        if ($('input[name="LeaveTypeId"]').val().length > 150) {
            if ($(".toast-error").length > 0 && $(".toast-error .toast-message").val() == errorMessage2) {
                $(".toast-error").remove()
            }
            toastr.error(errorMessage2)
        }
    })
      
    //Reason control
    $('textarea[name="Reason"]').focusout(function () {
        if ($('textarea[name="Reason"]').val().length > 150) {
            if ($(".toast-error").length > 0 && $(".toast-error .toast-message").text() == "Reason is not correct") {
                $(".toast-error").remove()
            }
            toastr.error("Reason is not correct")
        }
    })
        


    //modal
    $("#modal-add-depart").click(function () {
        $(".modal").css("display", "block")
        $(".modal-overlay").css("display", "block")
        $("body").css("overflow", "hidden")
    })

    //add input in add departmet form
    $(".add-input-btn").click(function () {

        if ($(".add-input-btn").closest(".edit-dep-form").length > 0) {
            var input = `  <div class="role-input-box">
                                <input type="text" name="Role"  class="form-control mb-3" placeholder="Role">
                            <a href=""class="delete-role-input">
                                <i class="fas fa-times"></i>
                            </a> 
                            </div>`;
        }
        else {
            var input = ` <input type="text" name="Role" class="form-control mb-2" placeholder="Role">`;
        }
        $(".role-inputs").append(input);

    })

    //

    if ($("#attendance-list").length > 0 || $(".report-main").length>0) {
       
    $('[data-toggle="tooltip"]').tooltip();

    }
    
    //Events
    $("#add-new-event").click(function () {
        $(".modal").css("display", "block")
        $(".modal-overlay").css("display", "block")
        $("body").css("overflow", "hidden")
    })

    if ($(".holiday-table")) {
        $(".dataTables_filter").parents().eq(1).remove();
    }

})


