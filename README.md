# AAValidationHelper
AAValidationHelper is a library that helps you easily writing validation html code in ASP.NET MVC + AngularJS. It uses DataAnnotations to do validation on client. This library is inspired by NgVal.

## Getting Started
1. Create an ASP.NET MVC project.
2. Add AngularJS and ngMessages module. (In order to use ngMessages, don't forget to attach the ngMessages module to your application module as a dependency.)
1. Using nuget to install **AAValidationHelper**.

    ``` PM> Install-Package AAValidationHelper ```
1. Create a .cshtml(for example: *ErrorTemplate.cshtml*) file in the Views/Shared folder of your project. 
    This file is used to generate ngMessages html code. You can use any valid HTML and Razor syntax to define your own error template. For example:

    ``` C#
    @model global::AAValidationHelper.ErrorMessageModel

    <div style="color: red" ng-messages="@(Model.FormName).@(Model.CtrlName).$error" ng-show="@(Model.FormName).@(Model.CtrlName).$dirty && @(Model.FormName).@(Model.CtrlName).$invalid" @Html.Raw(@Model.HtmlAttributes)>
        @foreach (var kv in Model.ErrorMessages)
        {
            <small ng-message="@kv.Key">@kv.Value</small>
        }
    </div>
    ```
    The ``` AAValidationHelper.ErrorMessageModel ``` contains everything about validation, so we need to use it as a model to generate html code.
1. Define your model with **DataAnnotations**.

    ``` C#
    public class Person
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public string Name { get; set; }

        [DisplayName("Age")]
        [Range(1, 150, ErrorMessage = "Please input valid {0}.({1}-{2})")]
        public int Age { get; set; }
    }
    ```
1. Now your can use AAValidationHelper's html extension methods(```NgValidationFor``` and ```NgMessageFor```) to easily write validation html code:

    ``` C#
    <form name="form">
        @Html.LabelFor(m => m.Name)
        <input type="text" name="username" ng-model="user.name" @Html.NgValidationFor(m => m.Name) />
        @Html.NgMessageFor("ErrorTemplate", "form", "username", m => m.Name)
        <br />
        @Html.LabelFor(m => m.Age)
        <input type="number" name="userage" ng-model="user.age" @Html.NgValidationFor(m => m.Age) />
        @Html.NgMessageFor("ErrorTemplate", "form", "userage", m => m.Age)
        <br />
    </form>
    ```

## About Sample
There is a sample web project using **AAValidationHelper** in src/Sample folder, it demostrates many uses, don't miss it.
