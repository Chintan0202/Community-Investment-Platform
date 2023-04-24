
tinymce.init({
    selector: '#editor',
    toolbar: ' bold italic strikethrough | superscript subscript removeformat'
});
const input = document.getElementById('image-upload');

const inputDiv = document.querySelector(".input-div")
const uploadFile = document.querySelector("#uploadFile")
const preview = document.querySelector("#preview")
let imagesArray = []
var img_src = []

uploadFile.addEventListener("change", () => {
    const files = uploadFile.files
    for (let i = 0; i < files.length; i++) {
        imagesArray.push(files[i])

    }
    console.log(imagesArray)
    displayImages()
});
inputDiv.addEventListener("drop", () => {
    e.preventDefault()
    const files = e.dataTransfer.files
    for (let i = 0; i < files.length; i++) {
        if (!files[i].type.match("image")) continue;

        if (imagesArray.every(image => image.name !== files[i].name))
            imagesArray.push(files[i])
    }
    displayImages()
})
function displayImages() {
    console.log("displaying image");

    var images = ""
    imagesArray.forEach((image, index) => {
        console.log(image)
        images += `<div class="image p-2">
        <img src="${image}" id="img_src"/>
        <span onclick="deleteImage(${index})">X</i></span>
        </div>`
    })
    preview.innerHTML = images;
}

function deleteImage(index) {
    imagesArray.splice(index, 1)
    displayImages()
    console.log(imagesArray)
}

var date;
$('#date-picker').on('change', function () {
    date = $(this).val();
    console.log($(this).val());
})

var missionid;

$('#select-tag').on('change', function () {
    console.log($('#select-tag').val());
    $.ajax({
        type: 'POST',

        url: '/Story/GetDraftStory',
        data: {

            missionid: $('#select-tag').val()
        },
        success:
            function (res) {
                console.log(res);
                console.log(res.description)

                console.log(res.date);
                $('.title').val(res.title);
                //For Date
                var date = res.date.split("T")[0];

                $('.publish-date').val(date);

                //For Description
                tinymce.activeEditor.setContent(res.description);
               
            },
        failure:
            function () {
                console.log('error');
            }
    });
});
