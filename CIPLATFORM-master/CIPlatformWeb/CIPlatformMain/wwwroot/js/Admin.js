var clockElement = document.getElementById('clock');
var userlist;
function clock() {
    clockElement.textContent = new Date().toString().substring(0, 25);
}

setInterval(clock, 1000);
function AddUser() {

    $.ajax({
        url: '/Admin/AddUser',

        success: function (res) {


            userlist = $(document).find("#UserData").html();
            console.log(userlist);

            var newbody = $($.parseHTML(res)).find("#AddUser").html();

            $("#DynamicDiv").html(newbody);

        }
    });
}
function GetUserData() {
    $('#v-pills-CMS-tab').removeClass('active-tab');
   
    $.ajax({
        url: '/Admin/UserData',

        success: function (res) {



            console.log(res);

            var newbody = $($.parseHTML(res)).html();

            $("#DynamicDiv").html(res);

            var table = new DataTable('#myTable', {

                "lengthChange": false,
                "ordering": false
            });
            $('#myInputTextField').keyup(function () {
                table.search($(this).val()).draw();
            });

        }
    });


}
function GetCMSList() {
    console.log("call");
    $('#UserList').removeClass('active-tab');
    $.ajax({
        url: '/Admin/CMSList',

        success: function (res) {


            $("#DynamicDiv").html(res);

            var CMSTable = new DataTable('#CMSTable', {

                "lengthChange": false,
                "ordering": false
            });

            $('#CMSSearch').keyup(function () {
                CMSTable.search($(this).val()).draw();
            });
        }
    });
}
function AddEditUser(userid) {
    $.ajax({
        url: '/Admin/AddEditUserPage',
        data: {
            UserId:userid
        },
        success: function (res) {

            
            $("#DynamicDiv").html(res);


        }
    });
}
function AddCMS() {
    
    $.ajax({
        url: '/Admin/AddEditCMSPage',

        success: function (res) {


            $("#DynamicDiv").html(res);

          
        }
    });
}
function EditCMS(CMSPageId) {
    
    $.ajax({
        url: '/Admin/AddEditCMSPage',
        data: {
            CMSId: CMSPageId
        },
        success: function (res) {


            $("#DynamicDiv").html(res);


        }
    });
}
function DeleteCMS(CMSPageId) {
    $.ajax({
        url: '/Admin/DeleteCMSPage',
        data: {
            CMSId: CMSPageId
        },
        success: function (res) {
            GetCMSList();
        }
    });
}
function GetMissionApplication() {
    $('#v-pills-CMS-tab').removeClass('active-tab');
    $('#UserList').removeClass('active-tab');

    $.ajax({
        url: '/Admin/MissionApplicationList',

        success: function (res) {



            console.log(res);

            var newbody = $($.parseHTML(res)).html();

            $("#DynamicDiv").html(res);

            let MissionAplicationtable = new DataTable('#MissionApplicationTable', {

                "lengthChange": false,
                "ordering": false
            });

            $('#SearchMissionApplication').keyup(function () {
                MissionAplicationtable.search($(this).val()).draw();
            });


        }
    });


}
function GetThemeList() {
    $('#v-pills-CMS-tab').removeClass('active-tab');
    $('#UserList').removeClass('active-tab');
    $.ajax({
        url: '/Admin/MissionThemeList',

        success: function (res) {
           

            $("#DynamicDiv").html(res);

            var ThemeTable = new DataTable('#ThemeTable', {

                "lengthChange": false,
                "ordering": false
            });

            $('#ThemeTableSearch').keyup(function () {
                ThemeTable.search($(this).val()).draw();
            });
        }
    });
}
function GetSkillsList() {
    $('#v-pills-CMS-tab').removeClass('active-tab');
    $('#UserList').removeClass('active-tab');
    $.ajax({
        url: '/Admin/MissionSkillList',

        success: function (res) {


            $("#DynamicDiv").html(res);

            var SkillTable = new DataTable('#SkillTable', {

                "lengthChange": false,
                "ordering": false
            });

            $('#SkillTableSearch').keyup(function () {
                SkillTable.search($(this).val()).draw();
            });
        }
    });
}
function FillModal(Skillid) {

    $('#SkillEdit').modal('show');
    $.ajax({
        url: '/Admin/DraftSkill',
        data: {
            SkillId: Skillid
        },
        success: function (res) {
            
            $('#SkillName').val(res.skillName);
            $('#SkillId').val(res.skillId);
        }
    });
    
}
function EditSkill() {
    
    $.ajax({
        url: '/Admin/EditSkill',
        data: {
            SkillId: $('#SkillId').val(),
            SkillName: $('#SkillName').val(),
            SkillStatus: $('#SkillStatus').val()
        },
        success: function (res) {
            $('#SkillEdit').modal('hide');
            GetSkillsList();
            
        }
    });
}
function DeleteSkill(Skillid) {
    $.ajax({
        url: '/Admin/DeleteSkill',
        data: {
            SkillId: Skillid
        },
        success: function (res) {
            $('#SkillEdit').modal('hide');
            GetSkillsList();
        }
    });
}
function GetStoryList() {
    $('#v-pills-CMS-tab').removeClass('active-tab');
    $('#UserList').removeClass('active-tab');
    $.ajax({
        url: '/Admin/StoryList',

        success: function (res) {


            $("#DynamicDiv").html(res);

            var StoryTable = new DataTable('#StoryTable', {

                "lengthChange": false,
                "ordering": false
            });

            $('#SearchStory').keyup(function () {
                StoryTable.search($(this).val()).draw();
            });
        }
    });
}
function GetStoryDetails(storyid) {
  
    $('#UserList').removeClass('active-tab');
    $.ajax({
        url: '/Admin/StoryDetails',
        data: {
            StoryId: storyid
        },
        success: function (res) {
            $("#DynamicDiv").html(res);
        }
    });
}
function DeleteStory(StoryId) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: '/Admin/StoryDelete',
                data: {
                    StoryId: StoryId
                },
                success: function (res) {

                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: 'Your work has been saved',
                        showConfirmButton: false,
                        timer: 3000
                    })
                    GetStoryList();
                }
            });

        }
    })



   
}
function ApproveStory(StoryId) {
    $.ajax({
        url: '/Admin/StoryApprove',
        data: {
            StoryId: StoryId
        },
        success: function (res) {

            Swal.fire({
                position: 'top-end',
                icon: 'success',
                title: 'Your work has been saved',
                showConfirmButton: false,
                timer: 3000
            })
            GetStoryList();
        }
    });
}
function DeclineStory(StoryId) {
    $.ajax({
        url: '/Admin/StoryDecline',
        data: {
            StoryId: StoryId
        },
        success: function (res) {

            Swal.fire({
                position: 'top-end',
                icon: 'success',
                title: 'Your work has been saved',
                showConfirmButton: false,
                timer: 3000
            })
            GetStoryList();
        }
    });
}
function GetMissionList() {
    $('#v-pills-CMS-tab').removeClass('active-tab');
    $('#UserList').removeClass('active-tab');
    $.ajax({
        url: '/Admin/MissionList',

        success: function (res) {


            $("#DynamicDiv").html(res);

            var MissionTable = new DataTable('#MissionTable', {

                "lengthChange": false,
                "ordering": false
            });

            $('#MissionSearch').keyup(function () {
                MissionTable.search($(this).val()).draw();
            });
        }
    });
}
function DeleteMission(MissionId) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: '/Admin/DeleteMission',
                data: {
                    MissionId: MissionId
                },
                success: function (res) {

                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: 'Your work has been saved',
                        showConfirmButton: false,
                        timer: 3000
                    })
                    GetMissionList();
                }
            });

        }
    })

}
function AddEditMission(missionid) {
    $.ajax({
        url: '/Admin/AddEditMission',
        data: {
            MissionId: missionid
        },
        success: function (res) {


            $("#DynamicDiv").html(res);


        }
    });
}

function DraftTheme(themeid) {
    $('#ThemeEdit').modal('show');
    $.ajax({
        url: '/Admin/DraftTheme',
        data: {
            MissionThemeId:themeid
        },
        success: function (res) {
            console.log(res);
            $('#ThemeName').val(res.title);
            $('#ThemeId').val(res.missionThemeId);
            $('#Status').val(res.status);
        }
    });
}

function AddEditTheme() {
    $.ajax({
        url: '/Admin/AddEditTheme',
        data: {
            ThemeId: $('#ThemeId').val(),
            ThemeName: $('#ThemeName').val(),
            Status: $('#Status').val()
        },
        success: function (res) {
            $('#ThemeEdit').modal('hide');
            GetThemeList();

        }
    });
}
