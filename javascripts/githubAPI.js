    
    loadProjectDescription("https://api.github.com/repos/joymon/joyful-visualstudio")
    loadReadMe(getGitHubAPIURL());
    function loadReadMe(url) {
        var contentContainerId = "#main_content";
        $.ajax({
            url: url,
            dataType: 'jsonp',
            success: function (response) {
                $(contentContainerId).html(GetHTMLFromMarkDown(GetStringFromBase64(response.data.content)));
            }
        });
    }
    function loadProjectDescription(url) {
        $.ajax({
            url: url,
            dataType: 'jsonp',
            success: function (response) {
                $("#desc").html(response.data.description);
            }
        });
    }
    function getGitHubAPIURL() {
        return 'https://api.github.com/repos/joymon/joyful-visualstudio/readme';
    }
    function GetStringFromBase64(base64) {
        return Base64.decode(base64);
        //return atob(base64)
    }
    function GetHTMLFromMarkDown(markdownString) {
        return marked(markdownString)
    }
