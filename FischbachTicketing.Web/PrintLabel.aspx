<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintLabel.aspx.cs" Inherits="FischbachTicketing.Web.PrintLabel" %>
<%@ Register assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Fischbach Print Label</title>
    <!-- Tell the browser to be responsive to screen width -->
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <!-- Font Awesome -->
    <link rel="stylesheet" href="Contents/plugins/fontawesome-free/css/all.min.css" />
    <!-- Ionicons -->
    <link rel="stylesheet" href="Contents/dist/css/ionicons.min.css" />
    <!-- overlayScrollbars -->
    <link rel="stylesheet" href="Contents/dist/css/adminlte.css" />
    <link rel="stylesheet" href="Contents/dist/css/style.css" />
    <!-- Google Font: Source Sans Pro -->
    <link href="~/Contents/fonts/font.css" rel="stylesheet" />
    <link runat="server" rel="shortcut icon" href="Contents/dist/img/favicon.ico" type="image/x-icon" />
    <script type="text/javascript" lang="javascript">
        function cwPrint() {
            //$("#CrystalReportViewer1").find("iframe")[0].contentWindow.print();
            //$("#CrystalReportViewer1_toptoolbar_print").click();
            //$('.wizbutton').click();

            var dvReport = document.getElementById("dvReport");
                      var frame1 = dvReport.getElementsByTagName("iframe")[0];
                      if (navigator.appName.indexOf("Internet Explorer") != -1) {
                          frame1.name = frame1.id;
                          window.frames[frame1.id].focus();
                          window.frames[frame1.id].print();
                      }
                      else {
                          var frameDoc = frame1.contentWindow ? frame1.contentWindow : frame1.contentDocument.document ? frame1.contentDocument.document : frame1.contentDocument;                          
                         frameDoc.print();
            }

            return false;
	    }
    </script>
</head>
<body class="hold-transition sidebar-mini">
    <!-- Site wrapper -->
    <div class="wrapper">
        <!-- Navbar -->
        <nav class="main-header navbar navbar-expand navbar-white navbar-light">
            <ul class="navbar-nav" style="align-items: baseline;">
                <li class="nav-item">
                    <a class="nav-link" data-widget="pushmenu" href="#" role="button"><i class="fas fa-bars"></i></a>
                </li>
                <li class="nav-item d-none d-sm-inline-block">
                    <a href="#" class="nav-link"><h3><asp:Literal ID="ltrlSystemName" runat="server"></asp:Literal></h3></a>
                </li>                
            </ul>            
        </nav>
        <!-- /.navbar -->
        <!-- Main Sidebar Container -->
        <aside class="main-sidebar sidebar-dark-primary elevation-4">
            <!-- Brand Logo -->
            <!-- Sidebar -->
            <div class="sidebar">
                <!-- Sidebar user (optional) -->
                <div class="user-panel mt-3 pb-3 mb-3 d-flex">
                    <div class="image">
                        <img src="../../Contents/dist/img/person.png" class="img-circle elevation-2" alt="User Image" />
                    </div>
                    <div class="info">
                        <a href="#" class="d-block"><asp:Literal ID="ltrlUserFullName" runat="server"></asp:Literal></a>
                    </div>
                </div>                
            </div>
            <!-- /.sidebar -->
        </aside>
            <div class="content-wrapper">                
                <section class="content">
                    <form runat="server">
                        <div class="card card-danger">
                            <div class="card-header">
                                <h3 class="card-title">Print Label</h3>
                            </div>
                            <div class="card-body" style="padding-left:1.25em;">
                                <div class="row">
                                    <%--<div class="col-md-1">
                                        <div class="form-group" style="display:none;">
                                            <label>Printer Name:</label>
                                            <asp:DropDownList ID="cmbPrinter" runat="server" CssClass="form-control" Visible="false"></asp:DropDownList>
                                        </div>
                                    </div>--%>
                                    <div class="col-md-11">
                                        <div class="form-group">
                                            <label>&nbsp;</label><br />                                                                                     
                                            <asp:Button ID="btncwPrint" runat="server" CssClass="btn btn-danger" Text="Print" OnClientClick="return cwPrint();" />                                            
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-group" id="dvReport">
                                            <CR:CrystalReportViewer ID="CrystalReportViewer1" SeparatePages="false" runat="server" CssClass="form-control" AutoDataBind="true" ToolPanelView="None" HasPageNavigationButtons="True" HasToggleGroupTreeButton="false" HasToggleParameterPanelButton="false" DisplayToolbar="True" EnableDatabaseLogonPrompt="False" EnableParameterPrompt="False" Visible="false" />
                                        </div>                                        
                                    </div>
                                </div>
                            </div>
                        </div> 
                    </form>
                </section>
            </div>
            <footer class="main-footer">
                <strong>Copyright &copy; 2021-<%=DateTime.Now.Year%> <a href="http://www.arisoftglobal.com">Arisoft</a>.</strong> All rights
                reserved.
            </footer>
        </div>
        <!-- ./wrapper -->
        <!-- jQuery -->
        <script src="Contents/plugins/jquery/jquery.min.js"></script>
        <!-- Bootstrap 4 -->
        <script src="Contents/plugins/bootstrap/js/bootstrap.bundle.min.js"></script>
        <!-- AdminLTE App -->
        <script src="Contents/dist/js/adminlte.min.js"></script> 
    <script type="text/javascript" lang="javascript">        
        window.close();
    </script>
</body>
</html>
