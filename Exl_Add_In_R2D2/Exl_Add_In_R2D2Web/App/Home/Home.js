/// <reference path="../App.js" />

(function () {
    "use strict";

    // The initialize function must be run each time a new page is loaded
    Office.initialize = function (reason) {
        $(document).ready(function () {
            app.initialize();

            $('#get-data-from-selection').click(getDataFromSelection);
            $('#get-data-about-cat').click(loadDataAboutCat);
        });
    };

    // Reads data from current document selection and displays a notification
    function getDataFromSelection() {
        Office.context.document.getSelectedDataAsync(Office.CoercionType.Text,
            function (result) {
                if (result.status === Office.AsyncResultStatus.Succeeded) {
                    app.showNotification('The selected text is:', '"' + result.value + '"');
                } else {
                    app.showNotification('Error:', result.error.message);
                }
            }
        );
    }

    function loadDataAboutCat()
    {
        app.showNotification('it works');
        getJason();
      //  sendFeedback();
        //getCatData();
      //  insertData();
        //insertMatrix();
       // insertDataAboutCat();
    }
    function getJason()
    {
        
        var t1;
        $.ajax({
            type: "GET",
            dataType: "json",
            contentType: "application/json",
            url: 'http://r2d2.azure-api.net/rssjson',            
            success: function (result)
            {
                insertData(result);
            },
            error: function (error) {
                           }
        });
        
    }

 
    function insertData(obj) {

        // Create a TableData object.
        var data = new Office.TableData();
        //data.headers = ["Address", "CreatedAt", "CreatedBy", "Description", "ImageUrl", "ObjectType", "Title", "UrlId"];
        data.headers = ["Address", "CreatedAt", "CreatedBy", "Description"];

        var i = 0;
        for (; i < obj.length; i++)
            //data.rows += [obj[i].Address, obj[i].CreatedAt, obj[i].CreatedBy, obj[i].Description, obj[i].ImageUrl, obj[i].ObjectType, obj[i].Title, obj[i].UrlId];
            data.rows[i] = [obj[i].Address, obj[i].CreatedAt, obj[i].CreatedBy, obj[i].Description];
                   
       
        // Set the myTable in the document.
        
        Office.context.document.setSelectedDataAsync(
          data,
          { coercionType: Office.CoercionType.Table },
          function (asyncResult) {
              if (asyncResult.status == "failed") {
                  app.showNotification("Action failed with error: " + asyncResult.error.message);
              } else {
                  app.showNotification("Check out your new table, then click next to learn another API call.");
              }
          }
        );
        
    }
   
    function get()
    {
        var obj;
        $.getJSON("http://r2d2.azure-api.net/rssjson?callback=?", function (data) {
            // Get the element with id summary and set the inner text to the result.
            obj = $.parseJSON(data.rowData);
            
        });

        var res = obj;
    }
   
    function sendFeedback() {

            

        var dataToPassToService;

        $.ajax({
            url: 'http://r2d2.azure-api.net/rssjson',
            type: 'POST',
            data: JSON.stringify(dataToPassToService),
            contentType: 'application/json;charset=utf-8'
        }).done(function (data) {
            app.showNotification(data.Status, data.Message);
        }).fail(function (status) {
            app.showNotification('Error', 'Could not communicate with the server.');
        }).always(function () {
            $('.disable-while-sending').prop('disabled', false);
        });

        var t = dataToPassToService;
    }
    function getCatData() {
        $.ajax({

            url: 'http://r2d2.azure-api.net/rssjson',
            type: "GET",
            dataType: "jsonp",
            async: false,
            success: function (msg) {
                JsonpCallback(msg);
            },
            error: function () {
                ErrorFunction();
            }

        });

    }


    function JsonpCallback(json)
    {
        var obj = JSON.parse(json);

    }
})();