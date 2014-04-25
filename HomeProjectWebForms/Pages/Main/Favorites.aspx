<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Favorites.aspx.cs" Inherits="HomeProjectWebForms.Pages.Main.Favorites" Title="Избранное" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <%--    <link type="text/css" href="/Content/jquery.toastmessage.css" rel="stylesheet"/>--%>
    <link href="/Content/MainPageStyle.css" rel="stylesheet" /> 
    <link href="/Content/FavoritesStyle.css" rel="stylesheet" /> 
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:SqlDataSource ID="EmployeeDataSource" runat="server" 
                       ConnectionString="<%$ ConnectionStrings:CompanyConnectionString %>"  
                       SelectCommand="GetPersonFavorites"
                       SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:SessionParameter Name="UserGuid" SessionField="CurrentUser" DbType="Guid"/>
        </SelectParameters>
    </asp:SqlDataSource>
    <div class="favoriteLayout">
        <asp:UpdateProgress ID="UpdateProgress2" AssociatedUpdatePanelID="UpdatePanel2" runat="server">
            <ProgressTemplate>
                <div class="centerProgressBar" >
                    <img id="image" src="/Images/loading2.png"/>           
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="UpdatePanel2"  runat="server" >
            <ContentTemplate>
                <asp:GridView ID="GridView1" runat="server" DataSourceID="EmployeeDataSource" 
                              DataKeyNames="Id" AutoGenerateColumns="false"
                              CssClass="favoriteGridView">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <div class="itemLayout">
                                    <div>
                                        <asp:Image ID="Image1" CssClass="employeeImage" runat="server" AlternateText="Изображение"
                                                   ImageUrl='<%# "~/Handlers/ShowEmployeeImage.ashx?Id=" + Eval("Id") %>' 
                                                   Height="50px" Width="50px"/>
                                    </div>
                                    <div class="itemFirstColumn leftAlignment">
                            
                                        <%--                            ImageUrl='<%# GetImage(Convert.ToString(DataBinder.Eval(Container.DataItem, "isFavorite"))) %>'                                        --%>
                                        <asp:ImageButton ID="ImageButton1" runat="server" class="favoriteButton" 
                                                         CommandArgument='<%# Eval("Id") %>' 
                                                         ImageUrl="/Images/Premium_Fill.png"                                        
                                                         OnClick="AddFavoritesClicked" />
                                        <asp:HyperLink CssClass="blockObject favoritePersonName" ID="ProductLink" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# "~/Pages/Main/Employee.aspx?ID=" + Eval("Id") %>' />
                            
                                        <asp:Label CssClass="jobSpan" ID="DepartmentLabel" runat="server" Text='<%#Eval("Department") %>' />

                                        <asp:Label CssClass="jobSpan" ID="JobTitleLabel" runat="server" Text='<%#Eval("Title") %>' />
                            
                                        <%--<asp:Label CssClass="adressSpan" ID="PlaceLabel" runat="server" Text='<%#Eval("Region") + "," + Eval("Street") + "," + Eval("Edifice") %>' />--%>
                                    </div>
                                    <div class="itemSecondColumn" >
                                        <%--                                        <img class="images leftAlignment" src="/Images/Contact.png"/>                                    
                                        <asp:Label ID="Label2" CssClass="phones" runat="server" Text='<%#Eval("PhoneNumber") %>' />
                                        <img class="images leftAlignment" src="/Images/Phone.png"/>--%>
                                        <img id="Img1" class="images leftAlignment" runat="server" src="/Images/Contact.png" Visible='<%# CheckVisibility(Convert.ToString(GetUserPhoneNumbers(Eval("Id")))) %>'/>                                    
                                        <asp:Label ID="Label2" CssClass="phones" runat="server" Text='<%# GetUserPhoneNumbers(Eval("Id")) %>' Visible='<%# CheckVisibility(Convert.ToString(GetUserPhoneNumbers(Eval("Id")))) %>' />
                                        <img id="Img2" class="images leftAlignment" runat="server" Visible='<%# CheckVisibility(Convert.ToString(GetUserSpecificPhoneNumbers(Eval("Id")))) %>' src="/Images/Phone.png"/>
                                        <asp:Label ID="Label1" CssClass="phones" runat="server" Text='<%# GetUserSpecificPhoneNumbers(Eval("Id")) %>' Visible='<%# CheckVisibility(Convert.ToString(GetUserSpecificPhoneNumbers(Eval("Id")))) %>'/>
                                                                
                                        <img class="images leftAlignment" src="/Images/Email.png" />
                                        <asp:HyperLink ID="EmailAddressLink" CssClass="phones" runat="server" Text='<%#Eval("EMail") %>'  NavigateUrl='<%#"mailto:" + Eval("EMail") %>' />                                    
                                    </div>
                        
                                </div>
                        
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>  
            </ContentTemplate>
        </asp:UpdatePanel> 
    </div>


</asp:Content>