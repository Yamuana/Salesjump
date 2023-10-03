﻿using System;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Windows;
using System.IO;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.Text;
using iTextSharp.tool.xml;
using Bus_EReport;
using System.Net;

public partial class MIS_Reports_Sec_od_distweekwise : System.Web.UI.Page
{
    Decimal iTotLstCount1 = 0;
    Decimal iTotLstCount = 0;
    Decimal iTotLstCount2 = 0; 
    DataSet dsDr = null;
    DataSet dsDrr = null;
    DataSet getweek = null;
    string sCmd = string.Empty;
    string div_code = string.Empty;
    string distributor = string.Empty;
    string ProdCode = string.Empty;
    string ProdSaleUnit = string.Empty;
    string ProdName = string.Empty;
    string val = string.Empty;
    string search = string.Empty;
    string stockist = string.Empty;
    string Year = string.Empty;
    string Month = string.Empty;
    string Stock_name = string.Empty;
    string sf_name = string.Empty;
    string stype = string.Empty;
    string sCurrentDate = string.Empty;
    string Sf_Code_multiple = string.Empty;
    string MultiProd_Code = string.Empty;
    string tot_dr = string.Empty;
    string strFMonthName = string.Empty;
    int iCount = -1;
    DataSet dsDoc = null;
    int FYear = 0;
    int FMonth = 0;
    string subdivision = string.Empty;
    string sURL = string.Empty;
    string dtype = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        div_code = Session["div_code"].ToString();
        Month = Request.QueryString["Month"].ToString();
        Year = Request.QueryString["Year"].ToString();
        distributor = Request.QueryString["Stockist_Code"].ToString();
        FMonth = Convert.ToInt32(Month);
        FYear = Convert.ToInt32(Year);
        //stype = Request.QueryString["stype"].ToString();
        lblRegionName.Text = sf_name;
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
        strFMonthName = mfi.GetMonthName(Convert.ToInt16(Month)).ToString();
        subdivision = Request.QueryString["subdivision"].ToString();

        lblHead.Text = "Secondary Order Distributorwise Weekwise for the Month of " + strFMonthName + " " + FYear;

        lblIDMonth.Visible = false;
        lblIDYear.Visible = false;

        //if (stype == "1")
        //{

            FillSF_Leave_Type();
        //}
        //else if (stype == "2")
        //{
        //    Fillproduct_total();
        //}


    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        Session["ctrl"] = pnlContents;
        Control ctrl = (Control)Session["ctrl"];
        PrintWebControl(ctrl);
    }
    public static void PrintWebControl(Control ControlToPrint)
    {
        StringWriter stringWrite = new StringWriter();
        System.Web.UI.HtmlTextWriter htmlWrite = new System.Web.UI.HtmlTextWriter(stringWrite);
        if (ControlToPrint is WebControl)
        {
            Unit w = new Unit(100, UnitType.Percentage);
            ((WebControl)ControlToPrint).Width = w;
        }
        Page pg = new Page();
        pg.EnableEventValidation = false;
        HtmlForm frm = new HtmlForm();
        pg.Controls.Add(frm);
        frm.Attributes.Add("runat", "server");
        frm.Controls.Add(ControlToPrint);
        pg.DesignerInitialize();
        pg.RenderControl(htmlWrite);
        string strHTML = stringWrite.ToString();
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.Write(strHTML);
        HttpContext.Current.Response.Write("<script>window.print();</script>");
        HttpContext.Current.Response.End();

    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        string strFileName = Page.Title;
        string attachment = "attachment; filename='" + strFileName + "'.xls";
        Response.ClearContent();
        Response.AddHeader("content-disposition", attachment);
        Response.ContentType = "application/ms-excel";
        StringWriter sw = new StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(sw);
        HtmlForm frm = new HtmlForm();
        form1.Parent.Controls.Add(frm);
        frm.Attributes["runat"] = "server";
        frm.Controls.Add(pnlContents);
        frm.RenderControl(htw);
        Response.Write(sw.ToString());
        Response.End();
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {

    }
    protected void btnExport_Click(object sender, EventArgs e)
    {


        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                //this.Page.RenderControl(hw);
                this.RenderControl(hw);
                StringReader sr = new StringReader(sw.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
            }

        }
    }



    private void Fillproduct_total()
    {
        TableRow tr_det_head = new TableRow();
        tr_det_head.BorderStyle = BorderStyle.Solid;
        tr_det_head.BorderWidth = 1;
        tr_det_head.BackColor = System.Drawing.Color.FromName("#496a9a");
        tr_det_head.Style.Add("Color", "White");
        tr_det_head.BorderColor = System.Drawing.Color.Black;

        SalesForce sf = new SalesForce();


        getweek = sf.Secondary_sales_get_no_of_week(FYear, strFMonthName);

        TableCell leave_utility = new TableCell();
        leave_utility.BorderStyle = BorderStyle.Solid;
        leave_utility.BorderWidth = 1;
        leave_utility.Width = 80;

        leave_utility.ColumnSpan = 3;
        Literal lit_DR_utility = new Literal();
        lit_DR_utility.Text = "<center>Weekly View</center>";
        leave_utility.BorderColor = System.Drawing.Color.Black;
        leave_utility.Attributes.Add("Class", "rptCellBorder");
        leave_utility.Controls.Add(lit_DR_utility);
        tr_det_head.Cells.Add(leave_utility);

        int df = Convert.ToInt32(getweek.Tables[0].Rows[0].ItemArray.GetValue(0));

        int wee = df + 1;

        if (getweek.Tables[0].Rows.Count > 0)
        {
            for (int i = 1; i < wee; i++)
            {
                TableCell tc_catg_namee = new TableCell();
                tc_catg_namee.BorderStyle = BorderStyle.Solid;
                tc_catg_namee.BorderWidth = 1;
                //text-align: center;

                tc_catg_namee.ColumnSpan = 2;
                HyperLink lit_catg_namee = new HyperLink();
                lit_catg_namee.Text = " Week-" + i.ToString();
                dtype = "total";
                sURL = "rpt_purchase_view_daywise.aspx?weekno=" + i + " &FYear=" + FYear + "&FMonth=" + FMonth + "&distributor=" + distributor + "&Subdivision=" + subdivision + "&Dtype=" + dtype + "";
                lit_catg_namee.Attributes["onclick"] = "javascript:window.open('" + sURL + "' ,'','resizable=yes,toolbar=no,menubar=no,scrollbars=1,status=no,width=500,height=600,left=0,top=0,zoom=50%');";
                lit_catg_namee.Style.Add("Color", "Blue");

                lit_catg_namee.NavigateUrl = "#";

                tc_catg_namee.Attributes.Add("Class", "rptCellBorder");
                tc_catg_namee.HorizontalAlign = HorizontalAlign.Center;
                tc_catg_namee.Controls.Add(lit_catg_namee);
                tr_det_head.Cells.Add(tc_catg_namee);
            }


        }
        tbl.Rows.Add(tr_det_head);



        SalesForce dr = new SalesForce();
        dsDr = dr.Secondary_sales_productdetail(div_code, subdivision);


        if (dsDr.Tables[0].Rows.Count > 0)
        {
            TableRow tr_catg = new TableRow();
            tr_catg.BackColor = System.Drawing.Color.FromName("#496a9a");
            tr_catg.Style.Add("Color", "White");



            TableCell tc_det_head_SNo = new TableCell();
            tc_det_head_SNo.BorderStyle = BorderStyle.Solid;
            tc_det_head_SNo.BorderWidth = 1;
            Literal lit_det_head_SNo = new Literal();
            lit_det_head_SNo.Text = "<b>S.No</b>";
            tc_det_head_SNo.Attributes.Add("Class", "tblHead");
            tc_det_head_SNo.Controls.Add(lit_det_head_SNo);
            tc_det_head_SNo.HorizontalAlign = HorizontalAlign.Center;
            tr_catg.Cells.Add(tc_det_head_SNo);


            TableCell tc_det_head_doc = new TableCell();
            tc_det_head_doc.Width = 120;
            tc_det_head_doc.BorderStyle = BorderStyle.Solid;
            tc_det_head_doc.BorderWidth = 1;
            Literal lit_det_head_doc = new Literal();
            lit_det_head_doc.Text = "<b>Product Name</b>";
            tc_det_head_doc.Attributes.Add("Class", "tblHead");
            tc_det_head_doc.Controls.Add(lit_det_head_doc);
            tc_det_head_doc.HorizontalAlign = HorizontalAlign.Center;
            tr_catg.Cells.Add(tc_det_head_doc);

            TableCell tc_det_head_hq = new TableCell();
            tc_det_head_hq.Width = 70;
            tc_det_head_hq.BorderStyle = BorderStyle.Solid;
            tc_det_head_hq.BorderWidth = 1;
            Literal lit_det_head_hq = new Literal();
            lit_det_head_hq.Text = "<b>UOM</b>";
            tc_det_head_hq.Attributes.Add("Class", "tblHead");
            tc_det_head_hq.Controls.Add(lit_det_head_hq);
            tc_det_head_hq.HorizontalAlign = HorizontalAlign.Center;
            tr_catg.Cells.Add(tc_det_head_hq);
            if (getweek.Tables[0].Rows.Count > 0)
            {

                for (int i = 1; i < wee; i++)
                {
                    TableCell tc_det_head_OP = new TableCell();
                    tc_det_head_OP.BorderStyle = BorderStyle.Solid;
                    tc_det_head_OP.BorderWidth = 1;
                    Literal lit_det_head_OP = new Literal();
                    lit_det_head_OP.Text = "<b>Quantity</b>";
                    tc_det_head_OP.Attributes.Add("Class", "tblHead");
                    tc_det_head_OP.Controls.Add(lit_det_head_OP);
                    tc_det_head_OP.HorizontalAlign = HorizontalAlign.Center;
                    tr_catg.Cells.Add(tc_det_head_OP);

                    TableCell tc_det_head_Qua = new TableCell();
                    tc_det_head_Qua.Width = 70;
                    tc_det_head_Qua.BorderStyle = BorderStyle.Solid;
                    tc_det_head_Qua.BorderWidth = 1;
                    Literal lit_det_head_qua = new Literal();
                    lit_det_head_qua.Text = "<b>Value</b>";
                    tc_det_head_Qua.Attributes.Add("Class", "tblHead");
                    tc_det_head_Qua.Controls.Add(lit_det_head_qua);
                    tc_det_head_Qua.HorizontalAlign = HorizontalAlign.Center;
                    tr_catg.Cells.Add(tc_det_head_Qua);

                   

                   





                    tbl.Rows.Add(tr_catg);



                }
            }



            iCount = 0;
            foreach (DataRow drdoctor in dsDr.Tables[0].Rows)
            {
                TableRow tr_det_sno = new TableRow();
                TableCell tc_det_SNo = new TableCell();
                iCount += 1;
                Literal lit_det_SNo = new Literal();
                lit_det_SNo.Text = "<center>" + iCount.ToString() + "</center>";
                tc_det_SNo.Attributes.Add("Class", "tblRow");
                tc_det_SNo.BorderStyle = BorderStyle.Solid;
                tc_det_SNo.BorderWidth = 1;
                tc_det_SNo.Controls.Add(lit_det_SNo);
                tr_det_sno.Cells.Add(tc_det_SNo);

                TableCell tc_det_Sl_No = new TableCell();
                tc_det_Sl_No.Visible = false;
                Literal lit_det_Sl_No = new Literal();
                lit_det_Sl_No.Text = "&nbsp;" + drdoctor["Product_Detail_Code"].ToString();
                tc_det_Sl_No.BorderStyle = BorderStyle.Solid;
                tc_det_Sl_No.BorderWidth = 1;
                tc_det_Sl_No.Attributes.Add("Class", "rptCellBorder");
                tc_det_Sl_No.Controls.Add(lit_det_Sl_No);
                tr_det_sno.Cells.Add(tc_det_Sl_No);

                TableCell tc_det_usr = new TableCell();
                Literal lit_det_usr = new Literal();
                lit_det_usr.Text = "&nbsp;" + drdoctor["Product_Detail_Name"].ToString();
                tc_det_usr.BorderStyle = BorderStyle.Solid;
                tc_det_usr.BorderWidth = 1;

                tc_det_usr.Attributes.Add("Class", "rptCellBorder");
                tc_det_usr.Controls.Add(lit_det_usr);
                tr_det_sno.Cells.Add(tc_det_usr);

                TableCell tc_det_sfcode = new TableCell();

                Literal lit_det_sfcode = new Literal();
                lit_det_sfcode.Text = "&nbsp;" + drdoctor["Product_Sale_Unit"].ToString();
                tc_det_sfcode.BorderStyle = BorderStyle.Solid;
                tc_det_sfcode.BorderWidth = 1;
                tc_det_sfcode.Attributes.Add("Class", "rptCellBorder");
                tc_det_sfcode.Controls.Add(lit_det_sfcode);
                tr_det_sno.Cells.Add(tc_det_sfcode);
                string week = "4";

                if (getweek.Tables[0].Rows.Count > 0)
                {

                    for (int i = 1; i < wee; i++)
                    {
                        SalesForce sd = new SalesForce();
                        dsDrr = sd.purchase_disributor_view_weekise_details_total(div_code, FMonth, FYear, i, drdoctor["Product_Detail_Code"].ToString());
                        if (dsDrr.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow drrdoctor in dsDrr.Tables[0].Rows)
                            {


                                TableCell tc_det_dr_name = new TableCell();
                                Literal lit_det_dr_name = new Literal();
                                lit_det_dr_name.Text = drrdoctor["Rec_Qty"].ToString();
                                tc_det_dr_name.Attributes.Add("Class", "tblRow");
                                tc_det_dr_name.BorderStyle = BorderStyle.Solid;
                                tc_det_dr_name.BorderWidth = 1;
                                tc_det_dr_name.Controls.Add(lit_det_dr_name);
                                tr_det_sno.Cells.Add(tc_det_dr_name);


                                TableCell tc_det_dr_hq = new TableCell();
                                Literal lit_det_dr_hq = new Literal();
                                lit_det_dr_hq.Text = drrdoctor["VALUE"].ToString();
                                tc_det_dr_hq.Attributes.Add("Class", "tblRow");
                                tc_det_dr_hq.BorderStyle = BorderStyle.Solid;
                                tc_det_dr_hq.BorderWidth = 1;
                                tc_det_dr_hq.Controls.Add(lit_det_dr_hq);
                                tr_det_sno.Cells.Add(tc_det_dr_hq);
                            }
                        }
                        else
                        {


                            TableCell tc_det_dr_name = new TableCell();
                            Literal lit_det_dr_name = new Literal();
                            lit_det_dr_name.Text = "";
                            tc_det_dr_name.Attributes.Add("Class", "tblRow");
                            tc_det_dr_name.BorderStyle = BorderStyle.Solid;
                            tc_det_dr_name.BorderWidth = 1;
                            tc_det_dr_name.Controls.Add(lit_det_dr_name);
                            tr_det_sno.Cells.Add(tc_det_dr_name);


                            TableCell tc_det_dr_hq = new TableCell();
                            Literal lit_det_dr_hq = new Literal();
                            lit_det_dr_hq.Text = "";
                            tc_det_dr_hq.Attributes.Add("Class", "tblRow");
                            tc_det_dr_hq.BorderStyle = BorderStyle.Solid;
                            tc_det_dr_hq.BorderWidth = 1;
                            tc_det_dr_hq.Controls.Add(lit_det_dr_hq);
                            tr_det_sno.Cells.Add(tc_det_dr_hq);

                        }


                        tbl.Rows.Add(tr_det_sno);














                    }
                }

                tbl.Rows.Add(tr_det_sno);


            }

        }


    }
    private void FillSF_Leave_Type()
    {
        TableRow tr_det_head = new TableRow();
        tr_det_head.BorderStyle = BorderStyle.Solid;
        tr_det_head.BorderWidth = 1;
        tr_det_head.BackColor = System.Drawing.Color.FromName("#496a9a");
        tr_det_head.Style.Add("Color", "White");
        tr_det_head.BorderColor = System.Drawing.Color.Black;

        SalesForce sf = new SalesForce();


        getweek = sf.Secondary_sales_get_no_of_week(FYear, strFMonthName);

        TableCell leave_utility = new TableCell();
        leave_utility.BorderStyle = BorderStyle.Solid;
        leave_utility.BorderWidth = 1;
        leave_utility.Width = 80;

        leave_utility.ColumnSpan = 3;
        Literal lit_DR_utility = new Literal();
        lit_DR_utility.Text = "Weekly View";
        leave_utility.BorderColor = System.Drawing.Color.Black;
        leave_utility.Attributes.Add("Class", "rptCellBorder");
leave_utility.Style.Add("text-align","center");
        leave_utility.Controls.Add(lit_DR_utility);
        tr_det_head.Cells.Add(leave_utility);

        int df = Convert.ToInt32(getweek.Tables[0].Rows[0].ItemArray.GetValue(0));

        int wee = df + 1;

        if (getweek.Tables[0].Rows.Count > 0)
        {
            for (int i = 1; i < wee; i++)
            {
                TableCell tc_catg_namee = new TableCell();
                tc_catg_namee.BorderStyle = BorderStyle.Solid;
                tc_catg_namee.BorderWidth = 1;
                //text-align: center;

                tc_catg_namee.ColumnSpan = 3;
                HyperLink lit_catg_namee = new HyperLink();
                lit_catg_namee.Text = " Week-" + i.ToString();
                dtype = "value";
                sURL = "sec_od_distDaywise.aspx?weekno=" + i + " &FYear=" + FYear + "&FMonth=" + FMonth + "&distributor=" + distributor + "&Subdivision=" + subdivision + "&Dtype=" + dtype + "";
                lit_catg_namee.Attributes["onclick"] = "javascript:window.open('" + sURL + "' ,'','resizable=yes,toolbar=no,menubar=no,scrollbars=1,status=no,width=500,height=600,left=0,top=0,zoom=50%');";
                lit_catg_namee.Style.Add("Color", "White");

                lit_catg_namee.NavigateUrl = "#";

                tc_catg_namee.Attributes.Add("Class", "rptCellBorder");
                tc_catg_namee.HorizontalAlign = HorizontalAlign.Center;
                tc_catg_namee.Controls.Add(lit_catg_namee);
                tr_det_head.Cells.Add(tc_catg_namee);
            }


        }
        tbl.Rows.Add(tr_det_head);



        SalesForce dr = new SalesForce();
        dsDr = dr.Secondary_sales_productdetail(div_code, subdivision);


        if (dsDr.Tables[0].Rows.Count > 0)
        {
            TableRow tr_catg = new TableRow();
            tr_catg.BackColor = System.Drawing.Color.FromName("#496a9a");
            tr_catg.Style.Add("Color", "White");



            TableCell tc_det_head_SNo = new TableCell();
            tc_det_head_SNo.BorderStyle = BorderStyle.Solid;
            tc_det_head_SNo.BorderWidth = 1;
            Literal lit_det_head_SNo = new Literal();
            lit_det_head_SNo.Text = "<b>S.No</b>";
            tc_det_head_SNo.Attributes.Add("Class", "tblHead");
            tc_det_head_SNo.Controls.Add(lit_det_head_SNo);
            tc_det_head_SNo.HorizontalAlign = HorizontalAlign.Center;
            tr_catg.Cells.Add(tc_det_head_SNo);


            TableCell tc_det_head_doc = new TableCell();
            tc_det_head_doc.Width = 120;
            tc_det_head_doc.BorderStyle = BorderStyle.Solid;
            tc_det_head_doc.BorderWidth = 1;
            Literal lit_det_head_doc = new Literal();
            lit_det_head_doc.Text = "<b>Product Name</b>";
            tc_det_head_doc.Attributes.Add("Class", "tblHead");
            tc_det_head_doc.Controls.Add(lit_det_head_doc);
            tc_det_head_doc.HorizontalAlign = HorizontalAlign.Center;
            tr_catg.Cells.Add(tc_det_head_doc);

            TableCell tc_det_head_hq = new TableCell();
            tc_det_head_hq.Width = 70;
            tc_det_head_hq.BorderStyle = BorderStyle.Solid;
            tc_det_head_hq.BorderWidth = 1;
            Literal lit_det_head_hq = new Literal();
            lit_det_head_hq.Text = "<b>UOM</b>";
            tc_det_head_hq.Attributes.Add("Class", "tblHead");
            tc_det_head_hq.Controls.Add(lit_det_head_hq);
            tc_det_head_hq.HorizontalAlign = HorizontalAlign.Center;
            tr_catg.Cells.Add(tc_det_head_hq);
            if (getweek.Tables[0].Rows.Count > 0)
            {

                for (int i = 1; i < wee; i++)
                {


                    TableCell tc_det_head_OP = new TableCell();
                    tc_det_head_OP.BorderStyle = BorderStyle.Solid;
                    tc_det_head_OP.BorderWidth = 1;
                    Literal lit_det_head_OP = new Literal();
                    lit_det_head_OP.Text = "<b>Quantity</b>";
                    tc_det_head_OP.Attributes.Add("Class", "tblHead");
                    tc_det_head_OP.Controls.Add(lit_det_head_OP);
                    tc_det_head_OP.HorizontalAlign = HorizontalAlign.Center;
                    tr_catg.Cells.Add(tc_det_head_OP);
                   
                    TableCell tc_det_head_Qua = new TableCell();
                    tc_det_head_Qua.Width = 70;
                    tc_det_head_Qua.BorderStyle = BorderStyle.Solid;
                    tc_det_head_Qua.BorderWidth = 1;
                    Literal lit_det_head_qua = new Literal();
                    lit_det_head_qua.Text = "<b>Value</b>";
                    tc_det_head_Qua.Attributes.Add("Class", "tblHead");
                    tc_det_head_Qua.Controls.Add(lit_det_head_qua);
                    tc_det_head_Qua.HorizontalAlign = HorizontalAlign.Center;
                    tr_catg.Cells.Add(tc_det_head_Qua);

                   


                    TableCell tc_det_head_OPw = new TableCell();
                    tc_det_head_OPw.BorderStyle = BorderStyle.Solid;
                    tc_det_head_OPw.BorderWidth = 1;
                    Literal lit_det_head_OPw = new Literal();
                    lit_det_head_OPw.Text = "<b>Netweight value</b>";
                    tc_det_head_OPw.Attributes.Add("Class", "tblHead");
                    tc_det_head_OPw.Controls.Add(lit_det_head_OPw);
                    tc_det_head_OPw.HorizontalAlign = HorizontalAlign.Center;
                    tr_catg.Cells.Add(tc_det_head_OPw);



                    tbl.Rows.Add(tr_catg);



                }
            }



            iCount = 0;
            foreach (DataRow drdoctor in dsDr.Tables[0].Rows)
            {
                TableRow tr_det_sno = new TableRow();
                TableCell tc_det_SNo = new TableCell();
                iCount += 1;
                Literal lit_det_SNo = new Literal();
                lit_det_SNo.Text =  iCount.ToString() ;
                tc_det_SNo.Attributes.Add("Class", "tblRow");
                tc_det_SNo.BorderStyle = BorderStyle.Solid;
                tc_det_SNo.BorderWidth = 1;
tc_det_SNo.Style.Add("text-align","right");
                tc_det_SNo.Controls.Add(lit_det_SNo);
                tr_det_sno.Cells.Add(tc_det_SNo);

                TableCell tc_det_Sl_No = new TableCell();
                tc_det_Sl_No.Visible = false;
                Literal lit_det_Sl_No = new Literal();
                lit_det_Sl_No.Text = "&nbsp;" + drdoctor["Product_Detail_Code"].ToString();
                tc_det_Sl_No.BorderStyle = BorderStyle.Solid;
                tc_det_Sl_No.BorderWidth = 1;
                tc_det_Sl_No.Attributes.Add("Class", "rptCellBorder");
                tc_det_Sl_No.Controls.Add(lit_det_Sl_No);
                tr_det_sno.Cells.Add(tc_det_Sl_No);

                TableCell tc_det_usr = new TableCell();
                Literal lit_det_usr = new Literal();
                lit_det_usr.Text = "&nbsp;" + drdoctor["Product_Detail_Name"].ToString();
                tc_det_usr.BorderStyle = BorderStyle.Solid;
                tc_det_usr.BorderWidth = 1;

                tc_det_usr.Attributes.Add("Class", "rptCellBorder");
                tc_det_usr.Controls.Add(lit_det_usr);
                tr_det_sno.Cells.Add(tc_det_usr);

                TableCell tc_det_sfcode = new TableCell();

                Literal lit_det_sfcode = new Literal();
                lit_det_sfcode.Text = "&nbsp;" + drdoctor["Product_sale_Unit"].ToString();
                tc_det_sfcode.BorderStyle = BorderStyle.Solid;
                tc_det_sfcode.BorderWidth = 1;
                tc_det_sfcode.Attributes.Add("Class", "rptCellBorder");
                tc_det_sfcode.Controls.Add(lit_det_sfcode);
                tr_det_sno.Cells.Add(tc_det_sfcode);
          

                if (getweek.Tables[0].Rows.Count > 0)
                {
                    string tot_dr = string.Empty;

                    for (int i = 1; i < wee; i++)
                    {
                        SalesForce sd = new SalesForce();
                        dsDrr = sd.retail_disributor_view_weekise_details(div_code, FMonth, FYear, i, drdoctor["Product_Detail_Code"].ToString(), distributor);
                        if (dsDrr.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow drrdoctor in dsDrr.Tables[0].Rows)
                            {
                               
                                TableCell tc_det_dr_name = new TableCell();
                                tc_det_dr_name.HorizontalAlign = HorizontalAlign.Right;
                                Literal lit_det_dr_name = new Literal();
                                lit_det_dr_name.Text = drrdoctor["Quantity"] ==DBNull.Value ? drrdoctor["Quantity"].ToString(): Convert.ToDecimal( drrdoctor["Quantity"]).ToString("0.00");
                                tc_det_dr_name.Attributes.Add("Class", "tblRow");
                                tc_det_dr_name.BorderStyle = BorderStyle.Solid;
                                tc_det_dr_name.BorderWidth = 1;
                                tc_det_dr_name.Controls.Add(lit_det_dr_name);
                                tr_det_sno.Cells.Add(tc_det_dr_name);

                                TableCell tc_det_dr_hq = new TableCell();
                                tc_det_dr_hq.HorizontalAlign = HorizontalAlign.Right;
                                Literal lit_det_dr_hq = new Literal();
                                lit_det_dr_hq.Text = drrdoctor["VALUE"] ==DBNull.Value ? drrdoctor["VALUE"].ToString(): Convert.ToDecimal( drrdoctor["VALUE"]).ToString("0.00");  
                                tot_dr = drrdoctor["VALUE"].ToString();

                               
                                //iTotLstCount += Decimal.Parse(drrdoctor["VALUE"].ToString());
                                tc_det_dr_hq.Attributes.Add("Class", "tblRow");
                                tc_det_dr_hq.BorderStyle = BorderStyle.Solid;
                                tc_det_dr_hq.BorderWidth = 1;
                                tc_det_dr_hq.Controls.Add(lit_det_dr_hq);
                                tr_det_sno.Cells.Add(tc_det_dr_hq);

                               

                             

                                TableCell tc_det_dr_namew = new TableCell();
                                tc_det_dr_namew.HorizontalAlign = HorizontalAlign.Right;
                                Literal lit_det_dr_namew = new Literal();
                                lit_det_dr_namew.Text = drrdoctor["net_value"] ==DBNull.Value ? drrdoctor["net_value"].ToString(): Convert.ToDecimal( drrdoctor["net_value"]).ToString("0.00"); 
                                tot_dr = drrdoctor["net_value"].ToString();

                               
                                //iTotLstCount1 += Decimal.Parse(drrdoctor["net_value"].ToString());
                                tc_det_dr_namew.Attributes.Add("Class", "tblRow");
                                tc_det_dr_namew.BorderStyle = BorderStyle.Solid;
                                tc_det_dr_namew.BorderWidth = 1;
                                tc_det_dr_namew.Controls.Add(lit_det_dr_namew);
                                tr_det_sno.Cells.Add(tc_det_dr_namew);

                            }
                        }
                        else
                        {

                            TableCell tc_det_dr_namew = new TableCell();
                            tc_det_dr_namew.HorizontalAlign = HorizontalAlign.Right;
                            Literal lit_det_dr_namew = new Literal();
                            lit_det_dr_namew.Text = "";
                            tc_det_dr_namew.Attributes.Add("Class", "tblRow");
                            tc_det_dr_namew.BorderStyle = BorderStyle.Solid;
                            tc_det_dr_namew.BorderWidth = 1;
                            tc_det_dr_namew.Controls.Add(lit_det_dr_namew);
                            tr_det_sno.Cells.Add(tc_det_dr_namew);
                            TableCell tc_det_dr_name = new TableCell();
                            Literal lit_det_dr_name = new Literal();
                            lit_det_dr_name.Text = "";
                            tc_det_dr_name.Attributes.Add("Class", "tblRow");
                            tc_det_dr_name.BorderStyle = BorderStyle.Solid;
                            tc_det_dr_name.BorderWidth = 1;
                            tc_det_dr_name.Controls.Add(lit_det_dr_name);
                            tr_det_sno.Cells.Add(tc_det_dr_name);


                            TableCell tc_det_dr_hq = new TableCell();
                            Literal lit_det_dr_hq = new Literal();
                            lit_det_dr_hq.Text = "";
                            tc_det_dr_hq.Attributes.Add("Class", "tblRow");
                            tc_det_dr_hq.BorderStyle = BorderStyle.Solid;
                            tc_det_dr_hq.BorderWidth = 1;
                            tc_det_dr_hq.Controls.Add(lit_det_dr_hq);
                            tr_det_sno.Cells.Add(tc_det_dr_hq);

                        }


                        tbl.Rows.Add(tr_det_sno);
                      



                    }
                   

                }

                tbl.Rows.Add(tr_det_sno);


            }
           
        }

          TableRow tr_total = new TableRow();

            TableCell tc_Count_Total = new TableCell();
            tc_Count_Total.BorderStyle = BorderStyle.Solid;
            tc_Count_Total.BorderWidth = 1;

            Literal lit_Count_Total = new Literal();
            lit_Count_Total.Text = "Total";
            tc_Count_Total.Attributes.Add("style", "color:Red;font-weight:bold;");
            tc_Count_Total.Controls.Add(lit_Count_Total);
            tc_Count_Total.Font.Bold.ToString();
            tc_Count_Total.BackColor = System.Drawing.Color.White;
            tc_Count_Total.Attributes.Add("Class", "tbldetail_main");
            tc_Count_Total.ColumnSpan = 3;
            tc_Count_Total.Style.Add("text-align", "center");
            tc_Count_Total.Style.Add("font-family", "Calibri");
            tc_Count_Total.Attributes.Add("Class", "rptCellBorder");
            tc_Count_Total.Style.Add("font-size", "10pt");

            tr_total.Cells.Add(tc_Count_Total);




            if (getweek.Tables[0].Rows.Count > 0)
            {
                for (int i = 1; i < wee; i++)
                {
              
                    TableCell tc_tot_month = new TableCell();
                   // tc_tot_month.ColumnSpan = 2;
                    HyperLink hyp_month = new HyperLink();
                    //int iTotLstCount = 0;

                    foreach (DataRow drdoctor in dsDr.Tables[0].Rows)
                    {
                        SalesForce sd = new SalesForce();
                        dsDrr = sd.retail_disributor_view_weekise_details(div_code, FMonth, FYear, i, drdoctor["Product_Detail_Code"].ToString(), distributor);
                        if (dsDrr.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow drr1doctor in dsDrr.Tables[0].Rows)
                            {
                                string quantot;
                                string valtot;
                                string nettot;

                                quantot = drr1doctor["Quantity"].ToString();
                                if (quantot != "" && quantot != "0")
                                {
                                    iTotLstCount1 += (decimal.Parse(quantot));

                                }
                                valtot = drr1doctor["VALUE"].ToString();
                                if (valtot != "" && valtot!="0")
                                {
                                    iTotLstCount += (decimal.Parse(valtot));
                                   
                                }
                                nettot = drr1doctor["net_value"].ToString();
                                if (nettot != "" && nettot != "0")
                                {
                                    iTotLstCount2 += (decimal.Parse(nettot));

                                }

                               


                            }
                        }
                    }
                  //  stCrtDtaPnt += Convert.ToString(iTotLstCount) + "},";
                    if (iTotLstCount1 != 0)
                    {
                        hyp_month.Text = iTotLstCount1.ToString("0.00");
                    }
                    tc_tot_month.BorderStyle = BorderStyle.Solid;
                    tc_tot_month.BorderWidth = 1;
                    tc_tot_month.BackColor = System.Drawing.Color.White;
                    tc_tot_month.Width = 200;
                    tc_tot_month.Style.Add("font-family", "Calibri");
                    tc_tot_month.Style.Add("font-size", "10pt");
                    tc_tot_month.HorizontalAlign = HorizontalAlign.Right;
                    tc_tot_month.VerticalAlign = VerticalAlign.Middle;
                    tc_tot_month.Controls.Add(hyp_month);
                    tc_tot_month.Attributes.Add("style", "font-weight:bold;");
                    tc_tot_month.Attributes.Add("Class", "rptCellBorder");
                    tr_total.Cells.Add(tc_tot_month);


                    TableCell tc_tot_month2 = new TableCell();

                    HyperLink hyp_month2 = new HyperLink();
                    if (iTotLstCount != 0)
                    {
                        hyp_month2.Text = iTotLstCount.ToString("0.00");
                    }
                    tc_tot_month2.BorderStyle = BorderStyle.Solid;
                    tc_tot_month2.BorderWidth = 1;
                    tc_tot_month2.BackColor = System.Drawing.Color.White;
                    tc_tot_month2.Width = 200;
                    tc_tot_month2.Style.Add("font-family", "Calibri");
                    tc_tot_month2.Style.Add("font-size", "10pt");
                    tc_tot_month2.HorizontalAlign = HorizontalAlign.Right;
                    tc_tot_month2.VerticalAlign = VerticalAlign.Middle;
                    tc_tot_month2.Controls.Add(hyp_month2);
                    tc_tot_month2.Attributes.Add("style", "font-weight:bold;");
                    tc_tot_month2.Attributes.Add("Class", "rptCellBorder");
                    tr_total.Cells.Add(tc_tot_month2);



                               TableCell tc_tot_month1 = new TableCell();
                  
                    HyperLink hyp_month1 = new HyperLink();
                    if (iTotLstCount2 != 0)
                    {
                        hyp_month1.Text = iTotLstCount2.ToString("0.00");
                    }
                                tc_tot_month1.BorderStyle = BorderStyle.Solid;
                    tc_tot_month1.BorderWidth = 1;
                    tc_tot_month1.BackColor = System.Drawing.Color.White;
                    tc_tot_month1.Width = 200;
                    tc_tot_month1.Style.Add("font-family", "Calibri");
                    tc_tot_month1.Style.Add("font-size", "10pt");
                    tc_tot_month1.HorizontalAlign = HorizontalAlign.Right;
                    tc_tot_month1.VerticalAlign = VerticalAlign.Middle;
                    tc_tot_month1.Controls.Add(hyp_month1);
                    tc_tot_month1.Attributes.Add("style", "font-weight:bold;");
                    tc_tot_month1.Attributes.Add("Class", "rptCellBorder");
                    tr_total.Cells.Add(tc_tot_month1);
                    iTotLstCount = 0;
                            iTotLstCount1=0;

                            iTotLstCount2 = 0;




                }
                

            }

            tbl.Rows.Add(tr_total);

    }


}