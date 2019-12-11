// function taken from tinyMCE docs
let filePickerCb = function (cb) {
    var input = document.createElement('input');
    input.setAttribute('type', 'file');
    input.setAttribute('accept', 'image/*');


    input.onchange = function () {
        var file = this.files[0];

        var reader = new FileReader();
        reader.onload = function () {
            /*
              Note: Now we need to register the blob in TinyMCEs image blob
              registry. In the next release this part hopefully won't be
              necessary, as we are looking to handle it internally.
            */
            var id = 'blobid' + (new Date()).getTime();
            var blobCache = tinymce.activeEditor.editorUpload.blobCache;
            var base64 = reader.result.split(',')[1];
            var blobInfo = blobCache.create(id, file, base64);
            blobCache.add(blobInfo);

            /* call the callback and populate the Title field with the file name */
            cb(blobInfo.blobUri(), { title: file.name });
        };
        reader.readAsDataURL(file);
    };

    input.click();
};

let imgUploadHandler = function (blobInfo, success, failure) {
    var xhr, formData;
    xhr = new XMLHttpRequest();
    xhr.withCredentials = false;
    xhr.open('POST', '/Image/Add');
    xhr.onload = function () {
        var json;

        if (xhr.status != 200) {
            failure('HTTP Error: ' + xhr.status);
            return;
        }
        json = JSON.parse(xhr.responseText);

        if (!json || typeof json.location != 'string') {
            failure('Invalid JSON: ' + xhr.responseText);
            return;
        }
        success(json.location);
    };

    formData = new FormData();
    formData.append('ImageFile', blobInfo.blob(), fileName(blobInfo));
    xhr.send(formData);
};

let fileName = function (blobInfo) {
    let name = blobInfo.blob().name,
        filename = name.substring(0, name.lastIndexOf('.')),
        ext = name.substring(name.lastIndexOf('.'));

    return `${filename}${Date.now()}${ext}`;
}

let headingStyle = `
    text-align:center;
    font-family: 'Montserrat', sans-serif;
    text-transform: uppercase;
`;

let chapterTemplate = `
<section>
    <h1 style="${headingStyle}">Your chapter\'s title</h1>
    <hr />
    <br />

    <p>Contents</p>
</section >
`;

tinymce.init({
    // the textarea to be overriden
    selector: '#Content',
    plugins: ['image', 'toc', 'hr', 'template'],
    image_uploadtab: false,
    // allow only images
    file_picker_types: 'image',
    // enable the user to pick images from their computer
    file_picker_callback: filePickerCb,
    // customize the toolbar
    toolbar: 'toc | template | undo redo | styleselect | align | bold italic | image',
    images_upload_handler: imgUploadHandler,
    templates: [
        { title: 'Chapter', description: 'Insert a chapter in your article', content: chapterTemplate}
    ]

});

