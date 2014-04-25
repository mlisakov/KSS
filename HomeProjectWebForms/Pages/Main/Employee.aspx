<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" MasterPageFile="~/Site.Master" CodeBehind="Employee.aspx.cs" Inherits="HomeProjectWebForms.Pages.Main.Employee" %>
<%@ Import Namespace="HomeProjectWebForms.Helpers" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">

    <link href="/Content/EmployeeStyle.css" rel="stylesheet" />
	
    <script type="text/javascript">
        var _isWorkPhoneLoaded = 0;
        var _isMobilePhoneLoaded = 0;

        function addPhoneClick() {
            var $container = $("#collapsibleContainer");
            if ($container != undefined && $container.length > 0)
                $container.slideToggle();

            var $img = $("#workPhoneImage");
            if ($img != undefined && $img.length > 0) {
                $img.toggleClass("toggledImage");
                if ($img.hasClass("toggledImage"))
                    _isWorkPhoneLoaded = 1;
                else {
                    _isWorkPhoneLoaded = 0;
                }
            }
        }

        function addMobilePhoneClick() {
            var $container = $("#collapsibleContainerMobile");
            if ($container != undefined && $container.length > 0)
                $container.slideToggle();

            var $img = $("#mobilePhoneImage");
            if ($img != undefined && $img.length > 0) {
                $img.toggleClass("toggledImage");
                if ($img.hasClass("toggledImage"))
                    _isMobilePhoneLoaded = 1;
                else {
                    _isMobilePhoneLoaded = 0;
                }
            }
        }

        function imgLoad() {
            if (_isWorkPhoneLoaded == 1) {
                //work phone
                var $container = $("#collapsibleContainer");
                if ($container != undefined && $container.length > 0)
                    $container.show();

                var $img = $("#workPhoneImage");
                if ($img != undefined && $img.length > 0) {

                    if (!$img.hasClass("toggledImage"))
                        $img.addClass("toggledImage");
                }
            }
            if (_isMobilePhoneLoaded == 1) {
                //mobile phone
                var $containerMobile = $("#collapsibleContainerMobile");
                if ($containerMobile != undefined && $containerMobile.length > 0)
                    $containerMobile.show();

                var $imgMobile = $("#mobilePhoneImage");
                if ($imgMobile != undefined && $imgMobile.length > 0) {

                    if (!$imgMobile.hasClass("toggledImage"))
                        $imgMobile.addClass("toggledImage");
                }
            }
        }

        function hideAddingButtons(isAdmin) {

            var $img = $("#workPhoneImage");
            if ($img != undefined && $img.length > 0) {
                if (isAdmin == 0)
                    $img.hide();
            }

            var $imgMobile = $("#mobilePhoneImage");
            if ($imgMobile != undefined && $imgMobile.length > 0) {

                if (isAdmin == 0)
                    $imgMobile.hide();
            }
        }

        function showError(text) {
            alert(text);
        }
    </script>

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div class="progressBar">
                <img id="image" src="/Images/loading2.png" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:SqlDataSource ID="EmployeeDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>"
                       SelectCommand="GetEmployeeInfo" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:QueryStringParameter QueryStringField="ID" Name="UserGuid" />
            <asp:SessionParameter Name="UIUserGuid" SessionField="CurrentUser" DbType="Guid" />
        </SelectParameters>
        <UpdateParameters>
            <asp:QueryStringParameter QueryStringField="ID" Name="UserGuid" />
        </UpdateParameters>
    </asp:SqlDataSource>
    <div class="employeeLayout">

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:FormView ID="EmployeeFormView" runat="server" DataSourceID="EmployeeDataSource" DataKeyNames="Id"  CssClass="formView" OnItemCreated="EmployeeFormView_OnItemCreated">
                    <ItemTemplate>
                        <div class="employeeViewItemTemplate">
                            <div class="employeeViewItemTemplateFirstColumn">
                                <asp:Image ID="employeeImage" runat="server" AlternateText="Изображение"
                                           ImageUrl='<%# "~/Handlers/ShowEmployeeImage.ashx?Id=" + Eval("Id") %>' CssClass="personImage" />
                            </div>
                            <div class="employeeViewItemTemplateSecondColumn">
                                <div class="employeeRow">
                                    <p>Имя:</p>
                                    <asp:Label ID="EmployeeName" runat="server" Text='<%# GetBindingValue("Name") %>' CssClass="employeeRowSpan" />
                                </div>

                                <div class="employeeRow">
                                    <p>Дата рождения:</p>
                                    <asp:Label ID="EmployeeBirthDay" runat="server" Text='<%# GetBindingValue("BirthDay") %>'  CssClass="employeeRowSpan" />
                                </div>

                                <div class="employeeRow">
                                    <p>Должность: </p>
                                    <asp:Label ID="JobTitleLabel" runat="server" Text='<%#GetBindingValue("Title") %>' CssClass="employeeRowSpan" />
                                </div>
                                
                                <div class="employeeRow">
                                    <p>Филиал: </p>
                                    <asp:Label ID="DepartmentLabel" runat="server" Text='<%#GetBindingValue("Department") %>' CssClass="employeeRowSpan" />
                                </div>

                                <div class="employeeRow">
                                    <img id="workPhoneImage" class="addWorkPhoneImage" src="/Images/Add.png" title="Добавить новый телефон" onclick=" addPhoneClick() " onload=" imgLoad() "/>
                                    
                                    <p id="workPhoneParagraph">Рабочий телефон:</p>

                                    <asp:SqlDataSource ID="UserNumbersDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>"
                                                       SelectCommand="[GetUserPhoneNumbers]" 
                                                       UpdateCommand="select 1" 
                                                       DeleteCommand="select 1" 
                                                       InsertCommand="select 1" 
                                                       SelectCommandType="StoredProcedure" >
                                        <SelectParameters>
                                            <asp:QueryStringParameter QueryStringField="ID" Name="UserGuid" />
                                        </SelectParameters>
                                        <UpdateParameters>
                                            <asp:QueryStringParameter QueryStringField="ID" Name="UserGuid" />
                                        </UpdateParameters>
                                    </asp:SqlDataSource>

                                    <asp:GridView ID="UserPhonesGridView" 
                                                  AutoGenerateColumns="false" 
                                                  runat="server" 
                                                  DataSourceID="UserNumbersDataSource" 
                                                  DataKeyNames="Id"
                                                  onrowupdating="UpdatePhone"
                                                  CssClass="UserPhonesGridViewClass">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Местоположение">
                                                <ItemTemplate>
                                                    <p style="font-weight: normal; margin-right: 10px;"><%# Eval("LocationPhone") %> </p>
                                                    <p style="font-weight: normal; margin-right: 10px;"><%# Eval("Type") %> </p>
                                                </ItemTemplate>

                                                <EditItemTemplate>
                                                    
                                                    <asp:SqlDataSource ID = "DivisionDataSource" runat=server 
                                                                       ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" 
                                                                       SelectCommand = "select Id,Division from DivisionState where not ParentId is NULL
                                                                                        union select '00000000-0000-0000-0000-000000000000','-Выберите филиал-'"/>

                                                    <asp:SqlDataSource ID = "RegionDataSource" runat=server ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" SelectCommand = 
                                                        "select  distinct(mainTable.Id),((select Country from Locality where Id=mainTable.Id)+','
                                                        +(select Region from Locality where Id=mainTable.Id)+','
                                                        +(select Locality from Locality where Id=mainTable.Id)) as UserAddress
                                                        from Locality as mainTable
                                                        left join Location on mainTable.Id=Location.LocalityId
                                                        where Location.DivisionId=@param
                                                        union select '00000000-0000-0000-0000-000000000000','-Выберите регион-'"
                                                        >
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="DivisionDropDownList" Name="param" PropertyName="SelectedValue"/>
                                                        </SelectParameters>
                                                    </asp:SqlDataSource>
                                
                                                    <div class="employeeRow" style="margin-right: 10px;">                                    
                                                        <p id="editFilial">Филиал:</p>
                                                        <asp:DropDownList ID="DivisionDropDownList" runat="server" AutoPostBack="true" 
                                                                          DataSourceid="DivisionDataSource" DataTextField="Division" DataValueField="Id" 
                                                                          CssClass="emptyInput"  OnSelectedIndexChanged="DivisionDropDownList_OnSelectedIndexChanged" SelectedValue='<%#Eval("division") %>'/><%-- --%>
                                                    </div>

                                                    <div class="employeeRow">
                                                        <p id="editRegion">Регион:</p>
                                                        <asp:DropDownList ID="RegionList" runat="server" 
                                                                          DataSourceid="RegionDataSource" 
                                                                          AutoPostBack="true"
                                                                          DataTextField="UserAddress" 
                                                                          DataValueField="Id" 
                                                                          CssClass="emptyInput"  
                                                                          SelectedValue='<%#UpdateSelectedValue(DataBinder.Eval(Container.DataItem, "locality")) %>' /><%--SelectedValue='<%#DataBinder.Eval(Container.DataItem,"locality") %>'--%>
                                                    </div>
                                
                                                    <div class="employeeRow">
                                                        <p id="editStreet">Улица:</p>
                                                        <asp:TextBox ID="StreetTextBox" runat="server" Text='<%#Eval("Street") %>' CssClass="employeeRowSpan emptyInput" />
                                                    </div>
                                
                                                    <div class="employeeRow">
                                                        <p id="editHouse">Дом:</p>
                                                        <asp:TextBox ID="EdificeTextBox" runat="server" Text='<%#Eval("Edifice") %>' CssClass="employeeRowSpan emptyInput" />
                                                    </div>
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Телефон">
                                                <itemtemplate>
                                                    <p style="font-weight: normal; margin-right: 10px;"><%# GUIHelper.ParsePhone(Eval("PhoneNumber").ToString(),Eval("Type").ToString(),Eval("CityPhoneCode").ToString()) %> </p>                                                                                                        
                                                </itemtemplate>
                                                <edititemtemplate>
                                                    <div class="employeeRow" style="margin-right: 10px;">                                    
                                                        <p id="editFilial">Тип:</p>
                                                        <asp:SqlDataSource ID = "PhoneTypeDataSource" runat=server 
                                                                           ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" 
                                                                           SelectCommand = "select Id,Type from PhoneType"/>
                                                        <asp:DropDownList ID="PhoneTypeDropDownList" runat="server" 
                                                                          DataSourceid="PhoneTypeDataSource" DataTextField="Type" DataValueField="Id" 
                                                                          CssClass="emptyInput"  SelectedValue='<%#Eval("phoneTypeId") %>'/><%-- --%>
                                                    </div>
                                                    <div class="employeeRow">
                                                        <p id="P5">Номер:</p>
                                                        <asp:textbox id="txtCompanyName1" runat="server" text='<%# GUIHelper.ParsePhone(Eval("PhoneNumber").ToString(),Eval("Type").ToString(),Eval("CityPhoneCode").ToString()) %>'
                                                                     CssClass="editPhoneBox" />
                                                    </div>
                                                </edititemtemplate>
                                            </asp:TemplateField>
                                            <asp:Templatefield>
                                                <itemtemplate>
                                                    <asp:ImageButton id="btnEdit1" runat="server" commandname="Edit" 
                                                                     ToolTip="Изменить" 
                                                                     Visible='<%# IsAdminMode() %>' 
                                                                     ImageUrl="~/Images/edit.png" 
                                                                     CssClass="imageButtons"/>
                                                    <asp:ImageButton id="btnDelete1" runat="server" commandname="Delete"                                                                      
                                                                     Visible='<%# IsAdminMode() %>'
                                                                     ImageUrl="~/Images/delete.png" 
                                                                     OnClientClick = " return confirm('Удалить номер?') "
                                                                     OnClick = "DeletePhone"
                                                                     CommandArgument = '<%# Eval("Id") %>'
                                                                     CssClass="imageButtons"
                                                                     ToolTip="Удалить"/>
                                                </itemtemplate>
                                                <edititemtemplate>
                                                    <asp:ImageButton id="btnUpdate1"  ImageUrl="~/Images/confirm.png"  runat="server"  CssClass="imageButtons" commandname="Update" ToolTip="Применить" />
                                                    <asp:ImageButton id="btnCancel1"  ImageUrl="~/Images/delete.png"  runat="server"  CssClass="imageButtons" commandname="Cancel" ToolTip="Отменить" />
                                                </edititemtemplate>
                                            </asp:Templatefield>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            Записей нет
                                        </EmptyDataTemplate>
                                    </asp:GridView>

                                    <div id="collapsibleContainer" class="CContainer">
										
                                        <p id="addWorkPhoneParagraph">Добавление рабочего телефона</p>
										
                                        <div class="collapsibleLayout">
                                            <div class="collapsibleFirstColumn">
                                                <div class="collapsibleCell">
                                                    <p id="numberParagraph">Номер</p>
                                                    <asp:TextBox ID="txtContactName" runat="server" TabIndex="1"  CssClass="contactName"/> 
                                                </div>
												      
                                                <div >
                                                    <asp:SqlDataSource ID="DivisionDataSource2" 
                                                                       runat=server 
                                                                       ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" 
                                                                       SelectCommand = "select Id,Division from DivisionState where not ParentId is NULL
                                                                                        union select '00000000-0000-0000-0000-000000000000','-Выберите филиал-'"/>
                                                    <p id="filialParagraph">Филиал</p>                                              
                                                    <asp:DropDownList ID="DivisionDropDownList2" runat="server" TabIndex="3" AutoPostBack="true" DataSourceid="DivisionDataSource2" DataTextField="Division" DataValueField="Id" 
                                                                      CssClass="phoneInput"  OnSelectedIndexChanged="DivisionDropDownList_OnSelectedIndexChanged2" SelectedValue='<%# new Guid(Session["CurrentUserDivision"].ToString()) %>'/>
                                                </div>

                                            </div>
                                            <div id="collapsibleSecondColumn">
                                                <div class="collapsibleCell">
												
                                                    <p id="phoneTypeParagraph">Тип телефона</p>
												
                                                    <asp:SqlDataSource ID = "SqlDataSource1" 
                                                                       runat=server 
                                                                       ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" 
                                                                       SelectCommand = "SELECT  Id,Type FROM PhoneType"/>
												
                                                    <asp:DropDownList ID="PhoneTypeDDL" runat="server" TabIndex="2" AutoPostBack="true" DataSourceid="SqlDataSource1" DataTextField="Type" DataValueField="Id" 
                                                                      CssClass="phoneInput" />											
                                                </div>
												            
                                                <div >
                                                    <asp:SqlDataSource ID = "RegionDataSource2" 
                                                                       runat=server 
                                                                       ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" 
                                                                       SelectCommand = 
                                                                           "select  distinct(Location.Id),((select Country from Locality where Id=mainTable.Id)+','
														    +(select Region from Locality where Id=mainTable.Id)+','
														    +(select Locality from Locality where Id=mainTable.Id)) as UserAddress
														    from Locality as mainTable
														    left join Location on mainTable.Id=Location.LocalityId
														    where Location.DivisionId=@param
                                                            union select '00000000-0000-0000-0000-000000000000','-Выберите регион-'">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="DivisionDropDownList2" Name="param" PropertyName="SelectedValue"/>
                                                        </SelectParameters>
                                                    </asp:SqlDataSource>                                        
                                                    <p id="regionParagraph">Регион:</p>
                                                    <asp:DropDownList ID="RegionList2" runat="server" TabIndex="4"  DataSourceid="RegionDataSource2" DataTextField="UserAddress" DataValueField="Id" CssClass="phoneInput"/>
                                                </div>										  
                                            </div>
                                        </div>

                                        <div style="text-align: right">
                                            <asp:Button ID="btnAddPhone" runat="server" Text="Добавить" OnClick = "AddNewPhone" />											
                                            <input type="button"  value="Отменить"  onclick=" addPhoneClick() "/>
                                        </div>
                                    </div>
                                </div>
								
                                <div class="employeeRow">
                                    <img id="mobilePhoneImage" class="addWorkPhoneImage" src="/Images/Add.png" title="Добавить новый телефон" onclick=" addMobilePhoneClick() " onload=" imgLoad() "/>
									
                                    <p id="mobilePhoneParagraph">Личный телефон:</p>
									
									                                    
                                    <asp:SqlDataSource ID="UserSpecificNumbersDataSource" runat="server" 
                                        ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>"
                                                       SelectCommand="[GetUserSpecificPhoneNumbers]" 
                                                       UpdateCommand="select 1" 
                                                       DeleteCommand="select 1" 
                                                       InsertCommand="select 1"
                                        SelectCommandType="StoredProcedure">
                                        <SelectParameters>
                                            <asp:QueryStringParameter QueryStringField="ID" Name="UserGuid" />
                                        </SelectParameters>
                                    </asp:SqlDataSource>
									
                                    <asp:GridView ID="GridView2" 
                                                  AutoGenerateColumns="false" 
                                                  runat="server" 
                                                  DataSourceID="UserSpecificNumbersDataSource" 
                                                  DataKeyNames="Id"
                                                  onrowupdating="UpdateSelfPhone"
                                                  CssClass="UserPhonesGridViewClass">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Местоположение">
                                                <ItemTemplate>
                                                    <p style="font-weight: normal; margin-right: 10px;"><%# Eval("LocationPhone") %> </p>
                                                    <p style="font-weight: normal; margin-right: 10px;"><%# Eval("Type") %> </p>                                                  
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                     <div class="employeeRow" style="margin-right: 10px;">                                    
                                                         <%# Eval("LocationPhone") %>
                                                         </div>
                                                    <div class="employeeRow" style="margin-right: 10px;">                                    
                                                        <p id="editFilial">Тип:</p>
                                                    <asp:SqlDataSource ID = "PhoneTypeDataSource5" runat=server 
                                                                           ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" 
                                                                           SelectCommand = "select Id,Type from PhoneType"/>
                                                        <asp:DropDownList ID="PhoneTypeDropDownList5" runat="server" 
                                                                          DataSourceid="PhoneTypeDataSource5" DataTextField="Type" DataValueField="Id" 
                                                                          CssClass="emptyInput"  SelectedValue='<%#Eval("phoneTypeId") %>'/>
                                                        </div>
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                             <asp:TemplateField HeaderText="Положение">
                                                <ItemTemplate>
                                                    <p style="font-weight: normal; margin-right: 10px;"><%# Eval("Position") %> </p>                                                    
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                   <%# Eval("Position")%>
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Телефон">
                                                <itemtemplate>
                                                   <p style="font-weight: normal; margin-right: 10px;"><%# GUIHelper.ParsePhone(Eval("PhoneNumber").ToString(),Eval("Type").ToString(),Eval("CityPhoneCode").ToString()) %> </p>                                                                                                        
                                                </itemtemplate>
                                                <edititemtemplate>
                                                    <asp:textbox id="txtCompanyName1" runat="server" text='<%# GUIHelper.ParsePhone(Eval("PhoneNumber").ToString(),Eval("Type").ToString(),Eval("CityPhoneCode").ToString()) %>'
                                                                 CssClass="editPhoneBox" />
                                                </edititemtemplate>
                                            </asp:TemplateField>
                                            <asp:Templatefield>
                                                <itemtemplate>
                                                    <asp:ImageButton id="btnEdit1" runat="server" commandname="Edit" 
                                                                     ToolTip="Изменить" 
                                                                     Visible='<%# IsAdminMode() %>' 
                                                                     ImageUrl="~/Images/edit.png" 
                                                                     CssClass="imageButtons"/>
                                                    <asp:ImageButton id="btnDelete1" runat="server" commandname="Delete"                                                                      
                                                                     Visible='<%# IsAdminMode() %>'
                                                                     ImageUrl="~/Images/delete.png" 
                                                                     OnClientClick = " return confirm('Удалить номер?') "
                                                                     OnClick = "DeleteSelfPhone"
                                                                     CommandArgument = '<%# Eval("Id") %>'
                                                                     CssClass="imageButtons"
                                                                     ToolTip="Удалить"/>
                                                </itemtemplate>
                                                <edititemtemplate>
                                                    <asp:ImageButton id="btnUpdate1"  ImageUrl="~/Images/confirm.png"  runat="server"  CssClass="imageButtons" commandname="Update" ToolTip="Применить" />
                                                    <asp:ImageButton id="btnCancel1"  ImageUrl="~/Images/delete.png"  runat="server"  CssClass="imageButtons" commandname="Cancel" ToolTip="Отменить" />
                                                </edititemtemplate>
                                            </asp:Templatefield>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            Записей нет
                                        </EmptyDataTemplate>
                                    </asp:GridView>
									
                                    <div id="collapsibleContainerMobile" class="CContainer">
                                        <p id="addMobilePhoneParagraph">Добавление личного телефона</p>
										
                                        <div class="collapsibleLayout">
                                            <div class="collapsibleFirstColumn">
                                                <div class="collapsibleCell">
                                                    <p id="P1">Номер</p>
                                                    <asp:TextBox ID="TextBox1" runat="server" TabIndex="1"  CssClass="contactName"/> 
                                                </div>
												      
                                                <div >
                                                    <asp:SqlDataSource ID="SqlDataSource2" 
                                                                       runat=server 
                                                                       ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" 
                                                                       SelectCommand = "select Id,Division from DivisionState where not ParentId is NULL
                                                                                        union select '00000000-0000-0000-0000-000000000000','-Выберите филиал-'"/>
                                                    <p id="P2">Филиал</p>                                              
                                                    <asp:DropDownList ID="DropDownList1" runat="server" TabIndex="3" AutoPostBack="true" DataSourceid="DivisionDataSource2" DataTextField="Division" DataValueField="Id" 
                                                                      CssClass="phoneInput"  OnSelectedIndexChanged="DivisionDropDownList_OnSelectedIndexChanged3" SelectedValue='<%# new Guid(Session["CurrentUserDivision"].ToString()) %>'/>
                                                </div>
                                            </div>
											
                                            <div>
                                                <div class="collapsibleCell">
												
                                                    <p id="p3">Тип телефона</p>
												
                                                    <asp:SqlDataSource ID = "SqlDataSource3" 
                                                                       runat=server 
                                                                       ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" 
                                                                       SelectCommand = "SELECT  Id,Type FROM PhoneType"/>
												
                                                    <asp:DropDownList ID="DropDownList2" runat="server" TabIndex="2" AutoPostBack="true" DataSourceid="SqlDataSource3" DataTextField="Type" DataValueField="Id" 
                                                                      CssClass="phoneInput" />											
                                                </div>
												
                                                <div >
                                                    <div >
                                                    <asp:SqlDataSource ID = "SqlDataSource4" 
                                                                       runat=server 
                                                                       ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>" 
                                                                       SelectCommand = 
                                                                           "select  distinct(Location.Id),((select Country from Locality where Id=mainTable.Id)+','
														    +(select Region from Locality where Id=mainTable.Id)+','
														    +(select Locality from Locality where Id=mainTable.Id)) as UserAddress
														    from Locality as mainTable
														    left join Location on mainTable.Id=Location.LocalityId
														    where Location.DivisionId=@param
                                                            union select '00000000-0000-0000-0000-000000000000','-Выберите регион-'">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="DropDownList1" Name="param" PropertyName="SelectedValue"/>
                                                        </SelectParameters>
                                                    </asp:SqlDataSource>                                        
                                                    <p id="P4">Регион:</p>
                                                    <asp:DropDownList ID="RegionList3" runat="server" TabIndex="4" DataSourceid="SqlDataSource4" DataTextField="UserAddress" DataValueField="Id" CssClass="phoneInput"/>
                                                    </div > 
                                                    <div >
                                                    <p id="P6">Положение:</p>
                                                    <asp:SqlDataSource ID="SpecificStaffSDS" runat="server" ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>"
                                                                       SelectCommand="select mainTable.Id,((mainTable.Position)+','+(select Department from DepartmentState where DepartmentState.Id=mainTable.DepartmentId)) as Name from SpecificStaff as mainTable where EmployeeId=@param">
                                                        <SelectParameters>
                                                            <asp:QueryStringParameter QueryStringField="ID" Name="param" />
                                                        </SelectParameters>
                                                    </asp:SqlDataSource>
                                                    <asp:DropDownList ID="SpecificStaffDDL" runat="server" TabIndex="2" DataSourceid="SpecificStaffSDS" DataTextField="Name" DataValueField="Id" 
                                                                      CssClass="phoneInput" />	
                                                          </div >	
                                                </div>	
                                            </div>
                                        </div>
										
                                        <div style="text-align: right">
                                            <asp:Button ID="Button1" runat="server" Text="Добавить" OnClick = "AddNewSelfPhone" />											
                                            <input type="button"  value="Отменить"  onclick=" addMobilePhoneClick() "/>
                                        </div>
                                    </div>
                                </div>

                                <div class="employeeRow">
                                    <p>Email:</p>
                                    <asp:HyperLink ID="EmailAddressLink" runat="server" Text='<%#Eval("EMail") %>' NavigateUrl='<%#"mailto:" + Eval("EMail") %>' CssClass="employeeRowSpan" />
                                </div>

                                <div class="employeeRow">
                                    <asp:ImageButton ID="ImageButton1" runat="server" class="favoriteButton"
                                                     CommandArgument='<%# Eval("Id") + ";" + Eval("isFavorite") %>'
                                                     ImageUrl='<%# GetImage(Convert.ToString(DataBinder.Eval(Container.DataItem, "isFavorite"))) %>'
                                                     OnClick="AddFavoritesClicked" />
                                </div>
                                
                                <div class="employeeRow editButton">
                                    <asp:HyperLink ID="HyperLink1" runat="server" Text="Назад" NavigateUrl="~/Pages/Main/MainPage.aspx" CssClass="employeeRowSpan" />
                                </div>
                            </div>
                            
                        </div>
                    </ItemTemplate>
                </asp:FormView>
            </ContentTemplate>
        </asp:UpdatePanel>


    </div>
</asp:Content>