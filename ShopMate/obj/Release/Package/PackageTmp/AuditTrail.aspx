<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditTrail.aspx.cs" Inherits="ShopMate.AuditTrail" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h2>Audit Trail</h2>
        <br />
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <br />
        <div id="logs" style="align-content:center;float:left">
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="AuditLogId" DataSourceID="Auditlog" ForeColor="#333333" GridLines="None" Height="243px" Width="887px">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:CommandField ShowSelectButton="True" />
                <asp:BoundField DataField="AuditLogId" HeaderText="AuditLogId" InsertVisible="False" ReadOnly="True" SortExpression="AuditLogId" />
                <asp:BoundField DataField="UserName" HeaderText="UserName" SortExpression="UserName" />
                <asp:BoundField DataField="EventDateUTC" HeaderText="EventDateUTC" SortExpression="EventDateUTC" />
                <asp:BoundField DataField="EventType" HeaderText="EventType" SortExpression="EventType" />
                <asp:BoundField DataField="TypeFullName" HeaderText="TypeFullName" SortExpression="TypeFullName" />
                <asp:BoundField DataField="RecordId" HeaderText="RecordId" SortExpression="RecordId" />
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#E9E7E2" />
            <SortedAscendingHeaderStyle BackColor="#506C8C" />
            <SortedDescendingCellStyle BackColor="#FFFDF8" />
            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
        </asp:GridView>
            </div>
        <div id="logdetails" style="float:right">
        <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="Id" DataSourceID="LogDetails" ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="PropertyName" HeaderText="PropertyName" SortExpression="PropertyName" />
                <asp:BoundField DataField="OriginalValue" HeaderText="OriginalValue" SortExpression="OriginalValue" />
                <asp:BoundField DataField="NewValue" HeaderText="NewValue" SortExpression="NewValue" />
                <asp:BoundField DataField="AuditLogId" HeaderText="AuditLogId" SortExpression="AuditLogId" />
            </Columns>
            <EditRowStyle BackColor="#7C6F57" />
            <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#E3EAEB" />
            <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F8FAFA" />
            <SortedAscendingHeaderStyle BackColor="#246B61" />
            <SortedDescendingCellStyle BackColor="#D4DFE1" />
            <SortedDescendingHeaderStyle BackColor="#15524A" />
        </asp:GridView>
            </div>
        <asp:SqlDataSource ID="Auditlog" runat="server" ConnectionString="<%$ ConnectionStrings:SIConnectionString %>" SelectCommand="SELECT * FROM [AuditLogs] ORDER BY [EventDateUTC]"></asp:SqlDataSource>
        <asp:SqlDataSource ID="LogDetails" runat="server" ConnectionString="<%$ ConnectionStrings:SIConnectionString %>" SelectCommand="SELECT * FROM [AuditLogDetails] WHERE ([AuditLogId] = @AuditLogId)">
            <SelectParameters>
                <asp:ControlParameter ControlID="GridView1" Name="AuditLogId" PropertyName="SelectedValue" Type="Int64" />
            </SelectParameters>
        </asp:SqlDataSource>
        <br />
    
    </div>
    </form>
</body>
</html>
