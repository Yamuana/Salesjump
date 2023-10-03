﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Bus_EReport;

public partial class MIS_Reports_Inv_Product_Sales_Report : System.Web.UI.Page
{
    string div_code = string.Empty;

    DataSet dsTP = null;
    DataSet dsSalesForce =null;
    string tot_dr = string.Empty;
    string tot_FWD = string.Empty;
    string tot_dcr_dr = string.Empty;
    string sCurrentDate = string.Empty;
    string sf_code = string.Empty;
    string sf_type = string.Empty;
    string MultiSf_Code = string.Empty;
    int mode = -1;
    DateTime ServerStartTime;
    DateTime ServerEndTime;
protected override void OnPreInit(EventArgs e)
    {
		base.OnPreInit(e);
		sf_type = Session["sf_type"].ToString();
    	if (sf_type == "3")
		{
    		this.MasterPageFile = "~/Master.master";
    	}
    	else if(sf_type == "2")
		{
    		this.MasterPageFile = "~/Master_MGR.master";
  		}
 		else if(sf_type == "1")
    	{
    		this.MasterPageFile = "~/Master_MR.master";
	 	}
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        sf_code = Session["sf_code"].ToString();
        sf_type = Session["sf_type"].ToString();

        if (sf_type == "3")
        {
            div_code = Session["div_code"].ToString();
        }
        else
        {
            div_code = Session["div_code"].ToString();
        }
        if (!Page.IsPostBack)
        {
            //menu1.Title = this.Page.Title;
            ServerStartTime = DateTime.Now;
            base.OnPreInit(e);
            //menu1.FindControl("btnBack").Visible = false;

            FillYear();
     fillsubdivision();
     //fillproduct();
     if (subdiv.Items.Count > 0)
     {
         subdiv.SelectedIndex = 1;
         subdiv_SelectedIndexChanged(sender, e);
     }
//if(subdiv.Items.Count>0)
//{
//subdiv.SelectedIndex=1;
//}
        }

    }
private void fillsubdivision()  
    {
        SalesForce sd = new SalesForce();
        dsSalesForce = sd.Getsubdivisionwise(div_code);
        if (dsSalesForce.Tables[0].Rows.Count > 0)
        {
            subdiv.DataTextField = "subdivision_name";
            subdiv.DataValueField = "subdivision_code";
            subdiv.DataSource = dsSalesForce;
            subdiv.DataBind();
 subdiv.Items.Insert(0, new ListItem("--Select--", "0"));

        }
    }
    private void FillYear()
    {
        TourPlan tp = new TourPlan();
        dsTP = tp.Get_TP_Edit_Year(div_code);
        if (dsTP.Tables[0].Rows.Count > 0)
        {
            for (int k = Convert.ToInt16(dsTP.Tables[0].Rows[0]["Year"]); k <= DateTime.Now.Year + 1; k++)
            {
                ddlFYear.Items.Add(k.ToString());
                ddlTYear.Items.Add(k.ToString());
                ddlFYear.SelectedValue = DateTime.Now.Year.ToString();
                ddlTYear.SelectedValue = DateTime.Now.Year.ToString();
            }
        }

        ddlFMonth.SelectedValue = DateTime.Now.Month.ToString();
        ddlTMonth.SelectedValue = DateTime.Now.Month.ToString();

    }
    protected void btnGo_Click(object sender, EventArgs e)
    {

        int FYear = Convert.ToInt32(ddlFYear.SelectedValue);
        int TYear = Convert.ToInt32(ddlTYear.SelectedValue);
        int FMonth = Convert.ToInt32(ddlFMonth.SelectedValue);
        int TMonth = Convert.ToInt32(ddlTMonth.SelectedValue);
        if (FMonth > TMonth && TYear == FYear)
        {
            ClientScript.RegisterStartupScript(GetType(), "Message", "<SCRIPT LANGUAGE='javascript'>alert('To Month must be greater than From Month');</script>");
            ddlTMonth.Focus();
        }
        else if (FYear > TYear)
        {
            ClientScript.RegisterStartupScript(GetType(), "Message", "<SCRIPT LANGUAGE='javascript'>alert('To Year must be greater than From Year');</script>");
            ddlTYear.Focus();
        }
        else
        {

            if (FYear <= TYear)
            {
                string sURL = string.Empty;

               // sURL = "rpt_sales_lost_purchase.aspx?&FMonth=" + ddlFMonth.SelectedValue.ToString() + "&FYear=" + ddlFYear.SelectedValue.ToString() + "&TMonth=" + ddlTMonth.SelectedValue.ToString() + "&TYear=" + ddlTYear.SelectedValue.ToString()+"&Subdivision="+ subdiv.SelectedValue.ToString();

                sURL = "rpt_Inv_Product_Sales_Report.aspx?&FMonth=" + ddlFMonth.SelectedValue.ToString() + "&FYear=" + ddlFYear.SelectedValue.ToString() + "&TMonth=" + ddlTMonth.SelectedValue.ToString() + "&TYear=" + ddlTYear.SelectedValue.ToString() + "&Subdivision=" + subdiv.SelectedValue.ToString() + "&ProductCode=" + DDL_Product.SelectedValue.ToString() + "&ProductName=" + DDL_Product.SelectedItem.ToString();

                string newWin = "window.open('" + sURL + "',null,'resizable=yes,toolbar=no,scrollbars=yes,menubar=no,status=no,width=800,height=600,left=0,top=0');";
                    ScriptManager.RegisterClientScriptBlock(this,GetType(), "pop", newWin, true);
            }
        }
    }

    private void fillproduct()
    {
        DDL_Product.DataSource = null;
        DDL_Product.Items.Clear();
        DDL_Product.Items.Insert(0, new ListItem("--Select--", "0"));

        SalesForce sd = new SalesForce();
        dsSalesForce = sd.GetProduct_subdivisionwise(div_code, subdiv.SelectedValue);
        if (dsSalesForce.Tables[0].Rows.Count > 0)
        {
            DDL_Product.DataTextField = "Product_Detail_Name";
            DDL_Product.DataValueField = "Product_Detail_Code";
            DDL_Product.DataSource = dsSalesForce;
            DDL_Product.DataBind();
            DDL_Product.Items.Insert(0, new ListItem("--Select--", "0"));

        }
    }
    protected void subdiv_SelectedIndexChanged(object sender, EventArgs e)
    {
        fillproduct();
    }
}