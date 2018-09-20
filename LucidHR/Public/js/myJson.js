
$(document).ready(function () {

    


    //Fill Roles oprions
    function fillRoles() {
        $("select[name='RoleId']").empty();
        $.ajax({
            url: "/Employee/Roles/" + $('select[name="DepartmentId"]').val(),
            dataType: "Json",
            type: "Get",
            data: {},
            success: function (response) {
                $("select[name='RoleId']").append(`<option value=` + 0 + `>Role</option>`)
                $.each(response, function (key, value) {
                    var option = `<option value=` + value.Id + `>` + value.Name + `</option>`
                    $("select[name='RoleId']").append(option)
                })
            }

        })
    }

    //Upload profile

    //================================================================================
    //upload profile img file
    //================================================================================
    $("#upload").change(function () {
        var fl = this.files.length;
        var fd = new FormData();
        //for (var x = 0; x < fl; x++) {
            fd.append("files", this.files[0]);
        //}
        $.ajax({
            url: "/employee/upload",
            type: "post",
            dataType: "json",
            cache: false,
            contentType: false,
            processData:false,
            data: fd,
            success: (function (response) {
                var link = "/Public" + response.data.fileUrl;
                var img = `<img src=` + link + `>`
                $(".uploaded-img-link").append(img);

                $(".uploaded-img-link").attr("href", link)
                $("#upload").prop("disabled", true)
                $("#upload-lbl").css("cursor", "not-allowed")
                var icon = `<a href="#" class="delete-uploaded" data-file=` + response.data.fileName + `><i class="fas fa-times-circle delete-uploaded"></i></a>`
                $(".img").append(icon)
            })
        })
    });


    //================================================================================
    //remove uploaded file
    //================================================================================
    $(document).on("click", ".img .delete-uploaded", function (e) {
        e.preventDefault();
        var file = $(this).attr("data-file");
        $this = $(this);
        $.ajax({
            url: "/employee/removefile?fileName="+file,
            type: "get",
            dataType: "json",
            success: function (response) {
                if (response.status == 200) {
                    $(".delete-uploaded").remove();
                    $(".img img").remove();
                    $("#upload").prop("disabled", false)
                    $("#upload-lbl").css("cursor", "pointer")
                }
            }
        })
    })
    
    //================================================================================
    //fill role options in add emp form
    //================================================================================
    $('select[name="DepartmentId"]').change(function () {

        fillRoles()
    })
    //============================================================
   //add form input controls
  //============================================================

    $('input[name="FullName"]').keydown(
        function (event) {
            var input = $(this).val();
            var words = input.split(' ')
            var w1Length = words[0].length
            var errorMessage = "Name and Surname must be less than 50 charactest";
            if (words[1] != undefined) {
                var w2Length = words[1].length
                if (w2Length > 50) {
                    if ($(".toast-error").length > 0 && $(".toast-error .toast-message").text() == errorMessage) {
                        $(".toast-error").remove()
                    }
                    toastr.error(errorMessage)
                }
            }
            var numberOfWords = input.split(' ').length;
            if (w1Length > 50) {
                if ($(".toast-error").length > 0 && $(".toast-error .toast-message").text() == errorMessage) {
                    $(".toast-error").remove()
                }
                toastr.error('Name and Surname must be less than 50 charactest')
            }
            
            if (numberOfWords == 2 && event.keyCode == 32) {
                var infoMessage = "Please,write only name and surname";
                if ($(".toast-info").length > 0 && $(".toast-info .toast-message").text() == infoMessage) {
                    $(".toast-info").remove()
                }
                toastr.info(infoMessage)
               
                return false;
            }
        }
    );

   
    //================================================================================
    //create emp
    //================================================================================
    $(".add-emp-btn").click(function (e) {
        e.preventDefault();
        var info = $("#add-emp").serializeArray();
        var data = {};
        for (var i = 0; i < info.length; i++) {
            data[info[i].name] = info[i].value;
        }
        if ($(".delete-uploaded").length > 0) {
            data.Profile = $(".delete-uploaded").data("file");
        }
        $.ajax({
            url: "/employee/create",
            type: "post",
            dataType: "json",
            data: data,
            success: function (response) {
                if (response.status == 200) {
                    console.log(response.status)
                    var table = $(".table").DataTable();

                    console.log("Dsfdsdf")
                    //add new emp tr
                    var empTr = ` <tr id="` + response.data.id + `">
                                    <td name="profile">
                                        <div class="pretty p-svg p-curve">
                                            <input type="checkbox" value="`+ response.data.id + `" name="emp-checkbox" />
                                            <div class="state p-warning">
                                                <!-- svg path -->
                                                <svg class="svg svg-icon" viewBox="0 0 20 20">
                                                    <path d="M7.629,14.566c0.125,0.125,0.291,0.188,0.456,0.188c0.164,0,0.329-0.062,0.456-0.188l8.219-8.221c0.252-0.252,0.252-0.659,0-0.911c-0.252-0.252-0.659-0.252-0.911,0l-7.764,7.763L4.152,9.267c-0.252-0.251-0.66-0.251-0.911,0c-0.252,0.252-0.252,0.66,0,0.911L7.629,14.566z"
                                                          style="stroke: white;fill:white;"></path>
                                                </svg>
                                                <label></label>
                                            </div>
                                        </div>
                                        <div class="profile-img">       
                                            <img src="/Public/Upload/Profiles/ProfileThumbnails/`+ response.data.profile + `" alt="">
                                        </div>
                                    </td>
                                    <td name="name-email">
                                       <h6>`+ data.FullName + `</h6>
                                    <span>`+ data.Email + `</span>
                                    </td>
                                    <td name="emp-id">
                                         <span>LA-`+ response.data.id + `</span>
                                    </td>
                                    <td name="phone">
                                            <span>
                                        `+ data.Phone + `
                                    </span>
                                    </td>
                                    <td name="date">
                                        <span>`+ data.JoinDate + `</span>
                                    </td>
                                    <td name="role">
                                         <span>`+ response.data.role + `</span>
                                    </td>
                                    <td>
                                        <a href="/employee/edit/` + response.data.id + `"  class="emp-edit"><i class="far fa-edit"></i></a>
                                        <a href="/employee/delete/` + response.data.id + `" class="emp-delete"><i class="far fa-trash-alt"></i></a>
                                    </td>
                                </tr>`


                    var rowHtml = empTr;

                    table.row.add($(rowHtml)).draw();


                    $(".delete-uploaded").remove();
                    $(".img img").remove();
                    $("#upload").prop("disabled", false)
                    $("#upload-lbl").css("cursor", "pointer")
                    $("#add-emp")[0].reset();
                    if ($(".add-emp-error").length > 0) {
                        $(".add-emp-error").remove()
                    }
                    //close modal
                    $(".modal").css("display", "none")
                    $(".modal-overlay").css("display", "none")
                    $("body").css("overflow", "auto")
                    toastr.success("You added new emlployee")


                }
                else if (response.status == 407 || response.status == 402 || response.status == 405 || response.status == 406 || response.status == 404) {
                    if ($(".toast-error").length > 0 && $(".toast-error .toast-message").text() == response.message) {
                        $(".toast-error").remove()
                    }
                    toastr.error(response.message)
                }
                else if (response.status == 403) {
                    if ($(".toast-info").length > 0 && $(".toast-info .toast-message").text() == response.message) {
                        $(".toast-info").remove()
                    }
                    toastr.info(response.message)
                }

            }
        })
    })

    //===========================================================================
    //edit emp
    //===========================================================================

    //1.fill form
    $(document).on("click", ".emp-edit", function (e) {
        e.preventDefault()
        var $this = $(this);
        var empId = $(this).closest('tr').find('input[name="emp-checkbox"]').val();


        $.getJSON("/employee/edit/" + empId, function (response) {

            if (response.status == 200) {
                //show modal
                $(".modal").css("display", "block")
                $(".modal-overlay").css("display", "block")
                $("body").css("overflow", "hidden")

                //show edit button
                $(".add-emp-btn").hide();
                $(".edit-emp-btn").show();

                //remove profile
                $(".delete-uploaded").remove();
                $(".img img").remove();
                $("#upload").prop("disabled", false)
                $("#upload-lbl").css("cursor", "pointer")
                //fill profile
                var link = "/Public/Upload/Profiles/" + response.data.profile;
                var img = `<img src=` + link + `>`
                $(".uploaded-img-link").append(img);
                $(".uploaded-img-link").attr("href", link)
                $("#upload").prop("disabled", true)
                $("#upload-lbl").css("cursor", "not-allowed")
                var icon = `<a href="#" class="delete-uploaded" data-file=` + response.data.profile + `><i class="fas fa-times-circle delete-uploaded"></i></a>`
                $(".img").append(icon)

                $('input[name="FullName"]').val(response.data.name + " " + response.data.surname);
                $('input[name="Email"]').val(response.data.email);
                $('input[name="Phone"]').val(response.data.phone);

                if (response.data.gender == true) {
                    $('input[name="Gender"]').attr('checked', false)
                    $('input[name="Gender"][value=' + 1 + ']').attr("checked", true);
                    $('input[name="Gender"]').parent().prevAll(".gender-text").text("Female")
                }
                else {
                    $('input[name="Gender"]').attr('checked', false)
                    $('input[name="Gender"][value=' + 0 + ']').attr("checked", true);
                    $('input[name="Gender"]').parent().prevAll(".gender-text").text("Male")
                }
                $('select[name="DepartmentId"] option[value=' + response.data.department + ']').prop("selected", true);
                var selectedId = response.data.role;

                //fill selected role
                $("select[name='RoleId']").empty();
                $.ajax({
                    url: "/Employee/Roles/" + $('select[name="DepartmentId"]').val(),
                    dataType: "Json",
                    type: "Get",
                    data: {},
                    success: function (response) {
                        $("select[name='RoleId']").append(`<option value=` + 0 + `>Role</option>`)
                        $.each(response, function (key, value) {
                            if (value.Id == selectedId) {
                                var option = `<option value=` + value.Id + ` selected>` + value.Name + `</option>`
                            }
                            else {
                                var option = `<option value=` + value.Id + `>` + value.Name + `</option>`
                            }
                            $("select[name='RoleId']").append(option)
                        })
                    }
                })
                //end fill selected role

                $('select[name="RoleId"] option[value=' + response.data.role + ']').prop("selected", true);
                $('input[name="Salary"]').val(response.data.salary);
                $('input[name="JoinDate"]').val(response.data.date);
                $('input[name="Facebook"]').val(response.data.fb != "" ? response.data.fb : null);
                $('input[name="Twitter"]').val(response.data.twitter != "" ? response.data.twitter : null);
                $('input[name="Linkedin"]').val(response.data.linkedin != "" ? response.data.linkedin : null);
                $('input[name="Instagram"]').val(response.data.instagram != "" ? response.data.instagram : null);
                $(".edit-emp-btn").attr("data-id", empId);
            }
            else if (response.status == 404) {
            }
        })
    })

    //2.edit emp
    $(document).on("click", ".edit-emp-btn", function (e) {

        e.preventDefault();
        var info = $("#add-emp").serializeArray();
        var data = {};
        for (var i = 0; i < info.length; i++) {
            data[info[i].name] = info[i].value;
        }
        data.Profile = $(".delete-uploaded").data("file");
        data.Id = $(".edit-emp-btn").attr("data-id");
        $.ajax({
            url: "/employee/edit",
            type: "post",
            dataType: "json",
            data: data,
            success: function (response) {
                if (response.status == 200) {

                    //reset forms
                    $(".delete-uploaded").remove();
                    $(".img img").remove();
                    $("#upload").prop("disabled", false)
                    $("#upload-lbl").css("cursor", "pointer")
                    $("#add-emp")[0].reset();
                    $(".add-uploaded")[0].reset();
                    if ($(".add-emp-error").length > 0) {
                        $(".add-emp-error").remove()
                    }
                    //close modal
                    $(".modal").css("display", "none")
                    $(".modal-overlay").css("display", "none")
                    $("body").css("overflow", "auto")

                    //show toast
                    toastr.success(response.message)

                    //set emp updated data
                    var updatedTd = $('tr[id=' + response.data.id + ']')
                    console.log(updatedTd)

                    updatedTd.find("td[name='name-email']").find("h6").text(response.data.name + " " + response.data.surname)
                    updatedTd.find("td[name='name-email']").find("span").text(response.data.email)
                    updatedTd.find("td[name='emp-id']").find("span").text(response.data.id)
                    updatedTd.find("td[name='phone']").find("span").text(response.data.phone)
                    updatedTd.find("td[name='date']").find("span").text(response.data.date)
                    updatedTd.find("td[name='role']").find("span").text(response.data.empRole)
                    var sourceImg = "/Public/Upload/Profiles/ProfileThumbnails/" + response.data.profile;
                    updatedTd.find("td[name='profile']").find(".profile-img img").attr("src", sourceImg)
                }
                else if (response.status == 407 || response.status == 402 || response.status == 405 || response.status == 406 || response.status == 404) {
                    if ($(".toast-error").length > 0 && $(".toast-error .toast-message").text() == response.message) {
                        $(".toast-error").remove()
                    }
                    toastr.error(response.message)
                }
                else if (response.status == 403) {
                    if ($(".toast-info").length > 0 && $(".toast-info .toast-message").text() == response.message) {
                        $(".toast-info").remove()
                    }
                    toastr.info(response.message)
                }
            }
        })
    })



    //delete element
    $(document).on("click", ".emp-delete", function (e) {
        $this = $(this);
        e.preventDefault();

        var chkArray = [];
        //var table = $(".table").DataTable();
        //var rows = table
        //    .rows()
        //    .remove()
        //    .draw();


        /* look for all checkboes that have a class 'chk' attached to it and check if it was checked */
        $('input[name="emp-checkbox"]:checked').each(function () {
            chkArray.push($(this).val());
        });
        if (chkArray.length == 0) {
            var id = $this.closest("tr").find($('input[name="emp-checkbox"]')).val();
            chkArray.push(id)
        }
        var url = "";
        for (var i = 0; i < chkArray.length; i++) {
            console.log(chkArray[i]);
            url += "id=" + chkArray[i] + "&";
        }


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
                $.ajax({
                    url: "/employee/delete?" + url,
                    type: "get",
                    dataType: "json",
                    success: (function (response) {
                        if (response.status == 200) {
                            var table = $(".table").DataTable();
                            for (var i = 0; i < chkArray.length; i++) {
                                var elem = $('input[name="emp-checkbox"][value=' + chkArray[i] + ']')
                                $('input[name="emp-checkbox"][value=' + chkArray[i] + ']');
                                $('input[name="emp-checkbox"][value=' + chkArray[i] + ']').closest("tr").addClass("selected")

                                table.row('.selected').remove().draw(false)

                                //elem.closest("tr").remove()


                                //$this.closest("tr").remove()
                            }
                            $('input[name="emp-checkbox"]').attr("checked", false)
                            $('#employee-checkbox-all').attr("checked", false)
                            swal(
                                'Deleted!',
                                response.message,
                                'success'
                            )
                        }
                        else {
                            toastr.error(response.message)
                        }
                    })
                })

                //console.log($this.attr("href"))


            }
        })
    })



    //=============================================================================================
    //To Do List
    //=============================================================================================
    $(".add-to-do").click(function () {
        $(".modal").css("display", "block")
        $(".modal-overlay").css("display", "block")
        $("body").css("overflow", "hidden")
    })

    $("#add-to-do").submit(function (e) {
        e.preventDefault();

        var data = $("#add-to-do").serializeArray();
        console.log(data.length)

        console.log($("#add-to-do").attr("action"))
        $.ajax({
            url: $("#add-to-do").attr("action"),
            type: 'post',
            dataType: "json",
            data: data,
            success: function (response) {
                if (response.status == 200) {
                    $("#add-to-do")[0].reset();

                    $(".pages-btns-box").empty()

                    for (var i = 0; i < response.data.page; i++) {
                        var btn = `<a href="home/pagenation?page=` + (i + 1) + `" class="btn btn-info todo-page">` + (i + 1) + `</a>`
                        $(".pages-btns-box").append(btn);
                    }
                    $.getJSON("home/pagenation?page=1", function (response) {
            $(".to-do-list").empty();
            $.each(response.list, function (key, value) {
                var isCompleted = value.isCompleted == true ? "line-through" : "";
                var checked = value.isCompleted == true ? "checked" : "";
                var list = `<li>
                            <div class="pretty p-svg p-curve to-do-checkbox">
                                <input name="list" type="checkbox" value="`+ value.id + `" ` + checked +` />
                                <div class="state p-warning">
                                    <!-- svg path -->
                                    <svg class="svg svg-icon" viewBox="0 0 20 20">
                                        <path d="M7.629,14.566c0.125,0.125,0.291,0.188,0.456,0.188c0.164,0,0.329-0.062,0.456-0.188l8.219-8.221c0.252-0.252,0.252-0.659,0-0.911c-0.252-0.252-0.659-0.252-0.911,0l-7.764,7.763L4.152,9.267c-0.252-0.251-0.66-0.251-0.911,0c-0.252,0.252-0.252,0.66,0,0.911L7.629,14.566z"
                                              style="stroke: white;fill:white;"></path>
                                    </svg>
                                    <label class=`+isCompleted+`>` + value.title +`</label>
                                </div>
                            </div>
                            <span>SCHEDULED FOR `+value.date+`</span>
                        </li>`;
                $(".to-do-list").append(list)
            })
        })
                    toastr.success(response.message)
                }
                else {
                    toastr.error(response.message)
                }
            }
        })
    })

  
         
    //=============================================================================================
    //To Do List
    //=============================================================================================
    $(".add-to-do").click(function () {
        $(".modal").css("display", "block")
        $(".modal-overlay").css("display", "block")
        $("body").css("overflow", "hidden")
    })


    $(".add-to-do-btn").click(function (e) {
        //e.preventDefault();

        var data = $("#add-to-do").serializeArray();
        console.log(data.length)

        console.log($("#add-to-do").attr("action"))
        $.ajax({
            url: "/home/createlist/",
            type: 'post',
            dataType: "json",
            data: data,
            success: function (response) {
                if (response.status == 200) {
                    $(".pages-btns-box").empty()
                    console.log("blees dsdsnd")
                    for (var i = 0; i < response.data.page; i++) {
                        var btn = `<a href="home/pagenation?page=` + (i + 1) + `" class="btn btn-info todo-page">` + (i + 1) + `</a>`
                        $(".pages-btns-box").append(btn);
                    }
                    $.getJSON("home/pagenation?page=1", function (response) {
                        $(".to-do-list").empty();
                        $.each(response.list, function (key, value) {
                            var isCompleted = value.isCompleted == true ? "line-through" : "";
                            var checked = value.isCompleted == true ? "checked" : "";
                            var list = `<li>
                            <div class="pretty p-svg p-curve to-do-checkbox">
                                <input name="list" type="checkbox" value="`+ value.id + `" ` + checked + ` />
                                <div class="state p-warning">
                                    <!-- svg path -->
                                    <svg class="svg svg-icon" viewBox="0 0 20 20">
                                        <path d="M7.629,14.566c0.125,0.125,0.291,0.188,0.456,0.188c0.164,0,0.329-0.062,0.456-0.188l8.219-8.221c0.252-0.252,0.252-0.659,0-0.911c-0.252-0.252-0.659-0.252-0.911,0l-7.764,7.763L4.152,9.267c-0.252-0.251-0.66-0.251-0.911,0c-0.252,0.252-0.252,0.66,0,0.911L7.629,14.566z"
                                              style="stroke: white;fill:white;"></path>
                                    </svg>
                                    <label class=`+ isCompleted + `>` + value.title + `</label>
                                </div>
                            </div>
                            <span>SCHEDULED FOR `+ value.date + `</span>
                        </li>`;
                            $(".to-do-list").append(list)
                        })
                    })
                    $(".modal").hide()
                    console.log($(".modal").css("display"))
                    toastr.success(response.message)
                   
                }
                else {
                    toastr.error(response.message)
                }
            }
        })
    })

    //todo pagenation
    $(document).on("click", ".todo-page", function (e) {
        e.preventDefault()

        $.getJSON($(this).attr("href"), function (response) {
            $(".to-do-list").empty();
            $.each(response.list, function (key, value) {
                var isCompleted = value.isCompleted == true ? "line-through" : "";
                var checked = value.isCompleted == true ? "checked" : "";
                var list = `<li>
                            <div class="pretty p-svg p-curve to-do-checkbox">
                                <input name="list" type="checkbox" value="`+ value.id + `" ` + checked + ` />
                                <div class="state p-warning">
                                    <!-- svg path -->
                                    <svg class="svg svg-icon" viewBox="0 0 20 20">
                                        <path d="M7.629,14.566c0.125,0.125,0.291,0.188,0.456,0.188c0.164,0,0.329-0.062,0.456-0.188l8.219-8.221c0.252-0.252,0.252-0.659,0-0.911c-0.252-0.252-0.659-0.252-0.911,0l-7.764,7.763L4.152,9.267c-0.252-0.251-0.66-0.251-0.911,0c-0.252,0.252-0.252,0.66,0,0.911L7.629,14.566z"
                                              style="stroke: white;fill:white;"></path>
                                    </svg>
                                    <label class=`+ isCompleted + `>` + value.title + `</label>
                                </div>
                            </div>
                            <span>SCHEDULED FOR `+ value.date + `</span>
                        </li>`;
                $(".to-do-list").append(list)
            })
        })
    })

    //delete list   
    $(".list-delete").click(function (e) {
        e.preventDefault();
        var link = $(this).attr("href") + "?";
        var url = "";
        $('input[name="list"]:checked').each(function () {
            url += "id=" + $(this).val() + "&";
        })


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
                $.ajax({
                    url: link + url,
                    type: "get",
                    dataType: "json",
                    success: function (response)    {
                        if (response.status == 200) {

                            $('input[name="list"]:checked').each(function () {
                                $(this).closest("li").remove();
                            })

                            //pagenation
                            $(".pages-btns-box").empty()

                            for (var i = 0; i < response.data.page; i++) {
                                var btn = `<a href="home/pagenation?page=` + (i + 1) + `" class="btn btn-info todo-page">` + (i + 1) + `</a>`
                                $(".pages-btns-box").append(btn);
                            }
                            //refresh first page
                            $.getJSON("home/pagenation?page=1", function (response) {
                                $(".to-do-list").empty();
                                $.each(response.list, function (key, value) {
                                    var isCompleted = value.isCompleted == true ? "line-through" : "";
                                    var checked = value.isCompleted == true ? "checked" : "";
                                    var list = `<li>
                            <div class="pretty p-svg p-curve to-do-checkbox">
                                <input name="list" type="checkbox" value="`+ value.id + `" ` + checked + ` />
                                <div class="state p-warning">
                                    <!-- svg path -->
                                    <svg class="svg svg-icon" viewBox="0 0 20 20">
                                        <path d="M7.629,14.566c0.125,0.125,0.291,0.188,0.456,0.188c0.164,0,0.329-0.062,0.456-0.188l8.219-8.221c0.252-0.252,0.252-0.659,0-0.911c-0.252-0.252-0.659-0.252-0.911,0l-7.764,7.763L4.152,9.267c-0.252-0.251-0.66-0.251-0.911,0c-0.252,0.252-0.252,0.66,0,0.911L7.629,14.566z"
                                              style="stroke: white;fill:white;"></path>
                                    </svg>
                                    <label class=`+ isCompleted + `>` + value.title + `</label>
                                </div>
                            </div>
                            <span>SCHEDULED FOR `+ value.date + `</span>
                        </li>`;
                                    $(".to-do-list").append(list)
                                })
                            })



                            swal(
                                'Deleted!',
                                response.message,
                                'success'
                            )

                        }
                        else {
                            toastr.error(response.message)
                        }



                    }
                })




            }
        })



    })

    //remove role input from deprtment edit form
    $(document).on("click", ".delete-role-input", function (e) {
        e.preventDefault();
        var $this = $(this);
        if ($this.attr("href") != "") {
            $.getJSON($this.attr("href"), function (response) {
                if (response.status == 200) {
                    $this.closest(".role-input-box").remove();
                }
                else {
                    if ($(".toast-error").length > 0 && $(".toast-error .toast-message").text() == response.message) {
                        $(".toast-error").remove()
                    }
               
                    toastr.error(response.message)
                }
            })
        }
        else{
            $this.closest(".role-input-box").remove();

        }
    })

     // delete department

    $(".delete-dep").click(function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
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


    //Attendance table crud
    $("#attendance-list .attn-day").click(function () {
        console.log($(this).attr("data-attn"))
        var $this = $(this);
        var day = $this.attr("data-day");
        var empId = $this.closest("tr").attr("data-id");
        var isAttn;
        if ($this.attr("data-attn") == "True") {
            $this.attr("data-attn", "False")
            isAttn = false;
        }
        else if ($this.attr("data-attn") == "False") {
            $this.attr("data-attn", "True");
            isAttn = true;
        }
        console.log(isAttn)
        $.getJSON("/employee/empattendance?id=" + empId + "&day=" + day + "&isAttn=" + isAttn, function (response) {
            console.log(response.status);
        })
    })


    //Holiday edit
    $(".hol-edit").click(function (e) {
        e.preventDefault();

        var id = $(this).closest("tr").attr("data-id");
        var url = "/holidays/edit?id=" + id;

        $(".modal").css("display", "block")
        $(".modal-overlay").css("display", "block")
        $("body").css("overflow", "hidden")
        $(".holiday-form")[0].reset();

        $(".modal-add-buttons .edit-hol-btn").show();
        $(".modal-add-buttons .add-hol-btn").hide();

        $.getJSON($(this).attr("href"), function (response) {
            if (response.status == 200) {
                $('input[name="Name"]').val(response.data.name)
                $('input[name="StartDate"]').val(response.data.startDate)
                $('input[name="EndDate"]').val(response.data.endDate)
                $(".holiday-form").attr("action",url)
            }
        })
    })

    //Holiday delete
    $(".hol-delete").click(function (e) {
        e.preventDefault();
        $this = $(this);
        var url = $this.attr("href");
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
    
    //Event Add
    //$("#event-add-form").submit(function (e) {
    //    e.preventDefault();
    //                        var data = $("#event-add-form").serializeArray();
    //                        $.ajax({
    //                            url: $(this).attr("action"),
    //                            dataType: "json",
    //                            type: "post",
    //                            data: data,
    //                            success: function (response) {
    //                                if (response.status == 200) {
    //                                    //
    //                                    var events = [];
    //                                    var currentdate = new Date();
    //                                    var defaultDate;
    //                                    class myEvent {
    //                                        constructor(title, start, end, className) {
    //                                            this.title = title,
    //                                                this.start = start,
    //                                                this.end = end,
    //                                                this.className = className
    //                                        }
    //                                    }
    //                                    var className;
    //                                    defaultDate = response.dateNow
    //                                    console.log(defaultDate)

    //                                    $.each(response.data.evnt, function (key, value) {
    //                                        var date = currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getMilliseconds();
    //                                        //console.log(date)
    //                                        //console.log(response.status)
    //                                        if (value.Type == "Optional") {
    //                                            className = "bg-dark"
    //                                        }
    //                                        else if (value.Type == "Important") {
    //                                            className = "bg-danger"
    //                                        }
    //                                        else {
    //                                            className = "bg-primary"
    //                                        }
    //                                        var event = new myEvent(value.Title, value.StartDate + "T" + value.StartTime, value.EndDate + "T" + value.EndTime, className)
    //                                        events.push(event);
    //                                    })
    //                                    $('#calendar').fullCalendar('refetchEvents').change();


    //                                    toastr.success(response.message)
    //                                }
    //                                else {
    //                                    toastr.error(response.message)
    //                                }
    //                            }
    //                        })

   
    //})


    //delete event
    $(".event-delete").click(function (element) {
        console.log("Dsds")
        element.preventDefault();
        var url = $(this).attr("data-link");

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
                window.location.replace(url);
            }

        })
    })

    })


