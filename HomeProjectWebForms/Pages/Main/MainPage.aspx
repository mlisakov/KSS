<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" MasterPageFile="~/Site.Master" CodeBehind="MainPage.aspx.cs" Title="Главная" 
         Inherits="HomeProjectWebForms.Pages.Main.MainPage" %>
<%@ Register TagPrefix="Controls" Namespace="HomeProjectWebForms.Controls" Assembly="HomeProjectWebForms" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <link href="/Content/MainPageStyle.css" rel="stylesheet" />        
    <script type="text/javascript" src="/Scripts/jquery-1.9.1.js"> </script>        
    <script type="text/javascript" src="/Scripts/jquery.cookie.js"> </script>
    <script type="text/javascript" src="/Scripts/jquery.collapsible.js"> </script>
    <script type="text/javascript" src="/Scripts/LocalScripts/MainPageScripts.js"> </script>     
    <link rel="stylesheet" href="../../Content/jquery-ui.css" />
    <script type="text/javascript" src="/Scripts/jquery-ui.js"> </script>    

    <script type="text/javascript">

        function BindEvents() {
            $(document).ready(function() {

                var datePickerSetting = {
                    closeText: 'Закрыть',
                    prevText: 'Предыдущий',
                    nextText: 'Следующий',
                    currentText: '',
                    monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
                        'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
                    monthNamesShort: ['Янв.', 'Фев.', 'Март', 'Апр.', 'Май', 'Июнь',
                        'Июль', 'Авг.', 'Сен.', 'Окт.', 'Ноя.', 'Дек.'],
                    dayNames: ['Воскресение', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'],
                    dayNamesShort: ['вс', 'пн', 'вт', 'ср', 'чт', 'пт', 'сб'],
                    dayNamesMin: ['В', 'П', 'Вт', 'С', 'Ч', 'П', 'С'],
                    weekHeader: 'Нед.',
                    dateFormat: 'dd.mm.yy',
                    firstDay: 1,
                    isRTL: false,
                    showMonthAfterYear: false,
                    yearSuffix: ''
                };

                $("#birthDayStartTextBox").datepicker(datePickerSetting);
                $("#birthDayFinishTextBox").datepicker(datePickerSetting);

                $("#additionalSearchButton").click(function() {

                    $("#collapsibleContainer").slideToggle();

                    var container = $("#MainContent_EmployeesListView_tblEmployees");
                    if (container.length > 0) {
                        if (container.hasClass("toggleEmptyClass")) {
                            container.animate({ 'top': '90px' });
                        } else {
                            container.animate({ 'top': '230px' });
                        }
                        container.toggleClass("toggleEmptyClass");
                    } else {
                        var emptyDiv = $("#emptyDataDiv");
                        if (emptyDiv.length > 0) {
                            if (emptyDiv.hasClass("toggleEmptyClass")) {
                                emptyDiv.animate({ 'height': '70%' });
                            } else {
                                emptyDiv.animate({ 'height': '10%' });
                            }
                            emptyDiv.toggleClass("toggleEmptyClass");
                        }
                    }


                    $(".searchButton :first").toggleClass("emptyCssClass");
                    $(".searchButton :last").toggleClass("emptyCssClass");

                });
            });
        }

        function SetInputValues(startDateValue, endDateValue) {
            var $startDate = $("#birthDayStartTextBox");
            if ($startDate != undefined && $startDate.length > 0)
                $startDate.val(startDateValue);

            var $endDate = $("#birthDayFinishTextBox");
            if ($endDate != undefined && $endDate.length > 0)
                $endDate.val(endDateValue);
        }
    </script>

    <asp:ScriptManager ID="ScriptManager1" runat="server"/>
    
    <div id="layout">
        <div class="leftColumn">
            <asp:UpdateProgress ID="UpdateProgress1" 
                                AssociatedUpdatePanelID="UpdatePanel1" 
                                runat="server">
                <ProgressTemplate>
                    <div class="progressBar" >
                        <img id="image" src="/Images/loading2.png"/>           
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <asp:UpdatePanel ID="UpdatePanel1" 
                             runat="server">
                <ContentTemplate>
                    <Controls:CustomTreeView  ID="DepartmentTree" 
                                              runat="server"  
                                              OnSelectedNodeChanged="DepartmentTree_SelectedNodeChanged" 
                                              ExpandImageUrl="~/Images/Expand.png"
                                              CollapseImageUrl="~/Images/Collapse.png">
                        <LeafNodeStyle CssClass="leafNode"  />
                        <NodeStyle CssClass="treeNode" />
                        <RootNodeStyle CssClass="rootNode" />
                        <SelectedNodeStyle CssClass="selectNode" />
                        <ParentNodeStyle CssClass="anotherNode" />
                    </Controls:CustomTreeView>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <div class="centerColumn">
            <asp:UpdateProgress ID="UpdateProgress2" 
                                AssociatedUpdatePanelID="UpdatePanel2" 
                                runat="server">
                <ProgressTemplate>
                    <div class="centerProgressBar" >
                        <img id="image" src="/Images/loading2.png"/>           
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <asp:SqlDataSource ID="CustomFilteredEmployeesDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>"  
                               SelectCommand="GetPersons" SelectCommandType="StoredProcedure">
            </asp:SqlDataSource>

            <asp:SqlDataSource ID="EmployeesDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>"  
                               SelectCommand="GetDepartmentStateEmployees" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:ControlParameter Name="DepartmentStateGuid" ControlID="DepartmentTree" PropertyName="SelectedValue"/>
                    <asp:SessionParameter Name="UserGuid" SessionField="CurrentUser" DbType="Guid"/>
                </SelectParameters>
            </asp:SqlDataSource>

            <asp:SqlDataSource ID="FilteredEmployeesDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>"  
                               SelectCommand="GetFilteredDepartmentStateEmployees" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:SessionParameter Name="UserGuid" SessionField="CurrentUser" DbType="Guid"/>
                    <asp:ControlParameter Name="EmployeeFilterText" ControlID="SearchTextBox" PropertyName="Text"/>                    
                </SelectParameters>
            </asp:SqlDataSource>

            <asp:UpdatePanel ID="UpdatePanel2"  runat="server" >
                <ContentTemplate>
                    <script type="text/javascript">
                        Sys.Application.add_load(BindEvents);
                    </script>
                    

                    <div  id="section">
                        <asp:Button ID="Button1" Text="найти" runat="server" OnClick="SimpleSearchClicked" CssClass="searchButton"  />
                        <asp:Button ID="Button2" Text="найти" runat="server" OnClick="AdditionalSearchClicked" CssClass="searchButton emptyCssClass" />
                        <span class="searchTextBoxContainer">
                            <asp:TextBox Id="SearchTextBox" type="search" runat="server" CssClass="searchTextBox"/>
                        </span>
                    </div>

                    <div id="additionalSearch">
                        <p id="additionalSearchButton">расширенный поиск</p>
                    </div>
                    <div id="collapsibleContainer" style="display: none;">                        
                        <div class="content" >
                            <p  class="additionalSearchFieldLabel">Подразделение</p>
                            <asp:TextBox ID="DepartmentTextBox" runat="server" CssClass="additionalSearchField"/>
                        </div>
                        <div class="content">
                            <p class="additionalSearchFieldLabel" >Должность</p>
                            <asp:TextBox ID="PositionTextBox" runat="server" CssClass="additionalSearchField"/>                            
                        </div>
                        <div class="content">                            
                            <asp:CheckBox ID="HasPhoneCheckBox" runat="server" Text="Указан телефон" CssClass="checkbox"/>                            
                        </div>
                        <div class="content">
                            <p class="additionalSearchFieldLabel">Дни рождения с</p>    
                            <input type="text"  id="birthDayStartTextBox" name="birthDayStartTextBox"  size="30" />
                            <p class="additionalSearchFieldLabel little">по</p>
                            <input type="text"  id="birthDayFinishTextBox" name="birthDayFinishTextBox" size="30" />
                        </div>
                    </div>
                    <div id="toolBar">
                        <asp:ImageButton ID="ImageButton2" ToolTip="Печать" runat="server"  ImageUrl="/Images/print.png" CssClass="toolBarItem" OnClick="PrintSearchResult"/>
                        <asp:ImageButton ID="ImageButton3" ToolTip="Экспорт в Excel" runat="server"  ImageUrl="/Images/Export.png" CssClass="toolBarItem" OnClick="ExportToExcel" />                        
                    </div>
                    
                    <asp:ListView runat="server" ID="EmployeesListView"      
                                  DataSourceID="EmployeesDataSource"                             
                                  DataKeyNames="Id" 
                                  OnDataBound="EmployeesListView_OnDataBound">
                        
                        <LayoutTemplate>
                            <div id="EmployeesListViewLayout">
                                <div cellpadding="2" runat="server" id="tblEmployees" 
                                     style="width: inherit">
                                    <div runat="server" id="itemPlaceholder" />
                                </div>

                                <asp:DataPager runat="server" OnPreRender="DataPager_OnPreRender" ID="DataPager" PageSize="9" PagedControlID="EmployeesListView" >
                                    <Fields>
                                        <asp:NumericPagerField
                                            ButtonCount="15"
                                            PreviousPageText="Назад"
                                            NextPageText="Вперед" /> 
                                    </Fields>
                                </asp:DataPager>
                            </div>
                        </LayoutTemplate>
                                                
                        <ItemTemplate>
                            <div class="itemLayout">
                                <div class="lyncStatus2">
                                    <img class="lyncStatusImg" src="/Images/LyncStatuses/empty.png" data-email='<%#Eval("EMail") %>' 
                                         onload='<%# "javascript:showLyncStatus(\"" + Eval("EMail") + "\", this);" %>'/>
                                </div>
                                <div class="buttonsColumn">
                                    <asp:ImageButton ID="ImageButton1" runat="server" class="favoriteButton withLeftMargin" 
                                                     CommandArgument='<%# Eval("Id") + ";" + Eval("isFavorite") %>' 
                                                     ImageUrl='<%# GetImage(Convert.ToString(DataBinder.Eval(Container.DataItem, "isFavorite"))) %>'                                        
                                                     OnClick="AddFavoritesClicked" />
                                    <img class="lyncPanelButton" 
                                         src="/Images/lync.png" onclick='<%# "javascript:showLyncPresencePopup(\"" + Eval("EMail") + "\",this );" %>' 
                                         onmouseout =" javascript:hideLyncPresencePopup(); " />
                                </div>
                                
                                <div class="itemFirstColumn leftAlignment firstColumnWithMargin">

                                    <asp:HyperLink CssClass="blockObject" ID="ProductLink" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# "~/Pages/Main/Employee.aspx?ID=" + Eval("Id") %>' />
                                    <asp:Label CssClass="jobSpan" ID="DivisionLabel" runat="server" Text='<%#Eval("Division") %>' />
                                    <asp:Label CssClass="jobSpan" ID="DepartmentLabel" runat="server" Text='<%#Eval("Department") %>' />
                                    <asp:Label CssClass="jobSpan" ID="JobTitleLabel" runat="server" Text='<%#Eval("Title") %>' />
                                    <%--<asp:Label CssClass="adressSpan" ID="PlaceLabel" runat="server" Text='<%#Eval("Region") + "," + Eval("Street") + "," + Eval("Edifice") %>' />--%>
                                </div>

                                <div class="itemSecondColumn" >
                                    <img class="images leftAlignment" runat="server" src="/Images/Contact.png" Visible='<%# CheckVisibility(Convert.ToString(GetUserPhoneNumbers(Eval("Id")))) %>'/>                                    
                                    <asp:Label ID="Label2" CssClass="phones" runat="server" Text='<%# GetUserPhoneNumbers(Eval("Id")) %>' Visible='<%# CheckVisibility(Convert.ToString(GetUserPhoneNumbers(Eval("Id")))) %>' />
                                    <img class="images leftAlignment" runat="server" Visible='<%# CheckVisibility(Convert.ToString(GetUserSpecificPhoneNumbers(Eval("Id")))) %>' src="/Images/Phone.png"/>
                                    <asp:Label ID="Label1" CssClass="phones" runat="server" Text='<%# GetUserSpecificPhoneNumbers(Eval("Id")) %>' Visible='<%# CheckVisibility(Convert.ToString(GetUserSpecificPhoneNumbers(Eval("Id")))) %>'/>
                                    <img class="images leftAlignment" src="/Images/Email.png" />
                                    <asp:HyperLink ID="EmailAddressLink" CssClass="phones" runat="server" Text='<%#Eval("EMail") %>'  NavigateUrl='<%#"mailto:" + Eval("EMail") %>' Visible='<%# CheckVisibility(Convert.ToString(DataBinder.Eval(Container.DataItem, "EMail"))) %>'/>                                                                        
                                </div>
                            </div>
                        </ItemTemplate>
                        
                        <%--                       Шаблон при отсутствии данных--%>
                        <EmptyDataTemplate>
                            <div id="emptyDataDiv">
                                <p id="emptyText">Данные отсутствуют</p>
                            </div>
                        </EmptyDataTemplate>     
                    </asp:ListView>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="ImageButton2" />
                    <asp:PostBackTrigger ControlID="ImageButton3" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        
        <div class="rightColumn">
            <asp:UpdateProgress ID="UpdateProgress3" AssociatedUpdatePanelID="UpdatePanel3" runat="server">
                <ProgressTemplate>
                    <div class="rightProgressBar" >
                        <img id="image" src="/Images/loading2.png"/>           
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>

            <asp:SqlDataSource ID="BirthdaysDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>"  
                               SelectCommand="GetFilteredBirthDayEmployees" SelectCommandType="StoredProcedure" OnSelecting="ds_Selecting">
                <SelectParameters>
                    <asp:Parameter Name="Interval"  DefaultValue="1"/>
                    <asp:SessionParameter Name="Division" SessionField="CurrentUserDivision" DbType="Guid"/>
                </SelectParameters>
            </asp:SqlDataSource>

            <asp:UpdatePanel ID="UpdatePanel3"  runat="server" >
                <ContentTemplate>
                    <div id="birthdayToolBar">
                        <asp:ImageButton ID="dayButton" ToolTip="Сегодня" runat="server"  ImageUrl="/Images/day.png" CssClass="toolBarItem" OnClick="dayButtonClicked"/>
                        <asp:ImageButton ID="weekButton" ToolTip="За неделю" runat="server"  ImageUrl="/Images/week.png" CssClass="toolBarItem" OnClick="weekButtonClicked"/>
                        <asp:ImageButton ID="monthButton" ToolTip="За месяц" runat="server"  ImageUrl="/Images/month.png" CssClass="toolBarItem" OnClick="monthButtonClicked"/>
                    </div>
                                 
                    <div id="birthdaysListLayout">
                        <asp:ListView runat="server" ID="BirthdaysList"      
                                      DataSourceID="BirthdaysDataSource"   
                                      DataKeyNames="Id"  >
                            <LayoutTemplate>
                                <div id="itemPlaceholder" runat="server" />
                            </LayoutTemplate>
                            <ItemTemplate>
                                <h5 class="groupHeader"><%# GetBirthdayGroupName("DepartmentId") %></h5>
                                <div class="birthdayItemDiv">
                                    <asp:HyperLink CssClass="blockObject" ID="ProductLink" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# "~/Pages/Main/Employee.aspx?ID=" + Eval("Id") %>' />
                                    <asp:Label CssClass="jobSpan" ID="JobTitleLabel" runat="server" Text='<%#Eval("Title") %>' />
                                    <%--                                     <asp:Label CssClass="jobSpan" ID="DepartmentLabel" runat="server" Text='<%#Eval("Department") %>' />--%>
                                    <div class="birthdayItemInnerDiv">
                                        <p>День рождения: </p>
                                        <asp:Label CssClass="dateSpan bolder" ID="BirthdayLableDay" runat="server" Text='<%#Eval("BDay") %>' />    
                                        <p class="bolder">.</p>
                                        <asp:Label CssClass="bolder" ID="BirthdayLableMonth" runat="server" Text='<%#Eval("BMonth") %>' />    
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>