
tinymce.init({
    selector: '#editor',
    toolbar: ' bold italic strikethrough | superscript subscript removeformat'
});


const inputDiv = document.querySelector(".input-div")
const input = document.querySelector("#StoryImages")
const output = document.querySelector("output")
let imagesArray = []




input.addEventListener("change", () => {
    const files = input.files
    for (let i = 0; i < files.length; i++) {
        imagesArray.push(files[i])
    }
    displayImages()
})

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
    let images = ""
    imagesArray.forEach((image, index) => {
        images += `<div class="image">
                <img src="${URL.createObjectURL(image)}" alt="image">
                <span onclick="deleteImage(${index})">&times;</span>
              </div>`
    })
    output.innerHTML = images
}

function deleteImage(index) {
    imagesArray.splice(index, 1)
    displayImages()
}
var formData = new FormData();
    $('#MissionId').on('change', function () {
       
        $.ajax({
            url: '/Story/GetStory',
            data: {
                MissionId :$('#MissionId').val()
            },
            success: function (res) {

                
                              if (res == "NoStoryFound") {
                                  console.log("hey");
                                
                                  $('#StoryTitle').val("");
                                  tinymce.activeEditor.setContent("");
                                  $('#StoryDate').val("");
                                  output.innerHTML = "";
                                  $('#StoryId').val("");
                                  formData.append('StoryId', 0);
                                }
                              else {
                                  res = JSON.parse(res);
                                  console.log(res);
                                  $('#StoryTitle').val(res.Title);
                                  tinymce.activeEditor.setContent(res.Description);
                                 
                                  $('#StoryId').val(res.StoryId);
                                  formData.append('StoryId', res.StoryId)
                                  var date = res.CreatedAt.split('T')[0];
                                  console.log(date);
                                  $('#StoryDate').val(date);
                                
                                  for (var i = 0; i < res.StoryMedia.$values.length; i++) {
                                      if (res.StoryMedia.$values[i].Type == "IMG") {
                                          formData.append('savedImages', res.StoryMedia.$values[i].Path);
                                      }

                                  }
                                
                                  var savedImages = "";
                                  for (var i = 0; i < res.StoryMedia.$values.length; i++) {
                                      console.log(i);
                                      
                                      if (res.StoryMedia.$values[i].Type == "URL") {
                                          $('#StoryVideoUrl').val(res.StoryMedia.$values[i].Path);
                                      }
                                      else {
                                          savedImages += `<div class="image">
                                                            <img src="${res.StoryMedia.$values[i].Path}" alt="image">
                                                            <span onclick="deleteImage(${i+5})">&times;</span>
                                                          </div>`

                                          output.innerHTML = savedImages;
                                      }
                                  }

                                  
                                 
                }
            }

        })
    })


function AddEditStory() {

 

    var editor = tinymce.get('editor');
    var Description = editor.getContent();
    var files = imagesArray;


    
        MissionId= $('#MissionId').val()
        StoryTitle= $('#StoryTitle').val()
        StoryDescription= Description
      
    StoryDate = $('#StoryDate').val()
    StoryUrl = $('#StoryVideoUrl').val()

   

    formData.append('MissionId', MissionId);
    formData.append('StoryDescription', StoryDescription);
    formData.append('StoryDate', StoryDate);
    formData.append('StoryTitle', StoryTitle);
    formData.append('StoryVideoURL', StoryUrl);

    var files = imagesArray;
    for (var i = 0; i < files.length; i++) {
        formData.append('StoryImages', files[i]);
    }

    $.ajax({
        type:'POST',
        url: '/Story/AddEditStoryPost',
        data: formData,
        contentType: false,
        processData: false,
        success: function (res) {
           

        }
    });
}