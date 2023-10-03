﻿<%@ Page Language="C#"  EnableEventValidation="false"  AutoEventWireup="true" CodeFile="rpt_retail_Distributor_wise_view.aspx.cs"Inherits="MIS_Reports_rpt_retail_Distributor_wise_view" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title> Retail Register Distributor Wise View</title>
  <link href="../css/bootstrap-3.3.2.min.css" rel="stylesheet" />
    <link href="../css/style.css" rel="stylesheet" />
    
     <script language="Javascript">
         function RefreshParent() {
            // window.opener.document.getElementById('form1').click();
             window.close();
         }
    </script>
</head>
<body>
    <form id="form1" runat="server">
   <asp:Panel ID="pnlbutton" runat="server">
        <table width="100%">
            <tr>
            
                <td width="60%" align="center" >
                <asp:Label ID="lblProd" runat="server" Text="Purchase Register -Distributor Wise" SkinID="lblMand" ></asp:Label>
                </td>
                 <td width="40%" align="right">
                        <table>
                            <tr>
<td>
                                 <asp:LinkButton ID="btnPrint" runat="Server" style="padding: 0px 20px;" class="btn btnPrint" OnClick="btnPrint_Click"/>
       <asp:LinkButton ID="btnExport"  runat="Server" style="padding: 0px 20px;" class="btn btnPdf"  OnClick="btnExport_Click" />
       <asp:LinkButton ID="btnExcel" runat="Server" style="padding: 0px 20px;" class="btn btnExcel" OnClick="btnExcel_Click"/>
       	<a name="btnClose" id="btnClose" type="button" style="padding: 0px 20px;" href="javascript:window.open('','_self').close();" class="btn btnClose"></a>
                   </td>
                            </tr>
                        </table>
                    </td>
            </tr>
        </table>
    </asp:Panel>
     <div>
    <center>
    <br />
    <br />
    </center>
    <center >
    <asp:Panel ID="pnlContents" runat="server" Width="100%">
        <table width="100%" align="center">
            <tbody>
            <tr>
          
              <td align="center" >
               <asp:Label ID="lblfieldname" runat="server" Font-Size="14px" Text="Distributor:" Font-Bold="true" ></asp:Label>
               <asp:Label ID="lblname" runat="server" SkinID="lblMand"></asp:Label>
              </td>
              
            </tr>
               
                                                                                                                    
                         
            </tbody>
        </table>
      </asp:Panel>
        <table width="100%" align="center">
                <tbody>
                    <tr>
                        <td align="center">
                            <asp:Table ID="tbl"  runat="server" class="newStly" GridLines="Both" Width="70%"/>
                           
                        </td>
                    </tr>
                </tbody>
            </table>  
       
        </center>
    </div>
    </form>
</body>
</html>