<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="HomeProjectWebForms.Contact" Title="Поддержка" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div style="background: white; height: 100%; width: 100%;">
        <hgroup class="title">
            <h1><%: Title %></h1>
            <h2>Контакты</h2>
        </hgroup>

        <section class="contact">
            <header>
                <h3>Телефоны:</h3>
            </header>
            <p>
                <span class="label">Тел.: </span>
                <span>25-77 (Для филиала "Калмэнерго" 5-77)</span>
            </p>
        </section>
        <section class="contact">
            <header>
                <h3>Email:</h3>
            </header>
            <p>
                <span class="label">Почта:</span>
                <span><a href="mailto:sd@mrsk-yuga.ru">sd@mrsk-yuga.ru</a></span>
            </p>
            <p>
                <span class="label">заявка в ServiceDesk:</span>
                <span><a href="http://sd.mrsk-yuga.local:8080">http://sd.mrsk-yuga.local:8080</a></span>
            </p>
        </section>
    </div>
</asp:Content>