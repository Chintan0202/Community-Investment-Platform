
//Add to Favrouite
function addtofav(favvalue, favmissionid) {
    console.log($(this).attr('id'));
    console.log(favvalue);
    console.log(favmissionid);
    $.ajax({
        type: 'POST',
        url: '/Home/Favrouite',
        data: {
            favvalue: favvalue,
            MissionId: favmissionid

        },
        success:

            function (res) {
               
              

                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true,
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                })

                Toast.fire({
                    icon: 'success',
                    title: 'Mission added to favrouite'
                })
                var newm = $($.parseHTML(res)).find("#card-cont").html();
                $("#card-cont").html(newm);

                var newl = $($.parseHTML(res)).find(".mission-list-cont").html();
                $(".mission-list-cont").html(newl);

                var newp = $($.parseHTML(res)).find(".page-links").html();
                $(".page-links").html(newp);
            },
        failure:
            function () {
                console.log('error');
            }
    });
}


//Remove From Favrouite
function removefav(favvalue, favmissionid) {
   
    console.log(favvalue);
    console.log(favmissionid);
    $.ajax({
        type: 'POST',
        url: '/Home/Favrouite',
        data: {
            favvalue: favvalue,
            MissionId: favmissionid

        },
        success:function (res) {
            
                
            var newm = $($.parseHTML(res)).find("#card-cont").html();
            $("#card-cont").html(newm);

            var newl = $($.parseHTML(res)).find(".mission-list-cont").html();
            $(".mission-list-cont").html(newl);

            var newp = $($.parseHTML(res)).find(".page-links").html();
            $(".page-links").html(newp);

            
            },
        failure:
            function () {
                console.log('error');
            }
    });
}