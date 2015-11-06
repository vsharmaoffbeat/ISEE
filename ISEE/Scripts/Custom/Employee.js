﻿@{
    ViewBag.Title = "employee";
    Layout = "~/Views/Shared/_Layout.cshtml";
}


@*<form role="form" method="post" action=""> *@
<div class="container top_employee_tab" style="background:#edecec;">

    <!---- top header start----->

    <div class="col-md-2 col-xs-12 ">
        <div class="form-group">
            <label for="usr">Last Name</label>
            <input class="form-control isee_input" id="" name="lastname" type="text" value="">
        </div>
    </div>

    <div class="col-md-2 col-xs-12">
        <div class="form-group">
            <label for="usr">First Name</label>
            <input class="form-control isee_input" id="" name="firstname" type="text" value="">
        </div>
    </div>

    <div class="col-md-2 col-xs-12">
        <div class="form-group">
            <label for="usr">Number</label>
            <input class="form-control isee_input" id="" maxlength="50" name="number" type="tel" value="">
        </div>
    </div>

    <div class="col-md-2 col-xs-12">
        <div class="form-group">
            <label for="usr">Manufacture</label>
            <select class="form-control tree_select_option isee_button_color" id="" maxlength="50" name="manufacture" value="">
                <option></option>
                <option>Clear</option>
                <option>Samsung</option>
                <option>LG</option>
                <option>BlackBerry</option>
                <option>Apple</option>
                <option>Motorola</option>
                <option>Nokia</option>
            </select>
        </div>
    </div>

    <div class="col-md-2 col-xs-12">
        <div class="form-group">
            <label for="usr">Phone Type</label>
            <select class="form-control tree_select_option isee_button_color" id="" name="phonetype" value="">
                <option></option>
                <option></option>
            </select>
        </div>
    </div>

    <div class="col-md-2 col-xs-12">
        <div class="form-group e_chkbox">
            <input type="checkbox" class="myClass" value="yes" id="" name="answer" data-color="green">
            <label for="usr">Active</label>
        </div>
        <button type="reset" class="btn btn-default pull-right e_pencil isee_button_color"><i class="fa fa-pencil"></i></button>
        <button type="submit" class="btn btn-default pull-right e_search isee_button_color"><i class="fa fa-search"></i></button>

    </div>


    <!---- top header end----->

</div>


<div class="container second_employee_window">


    <div class="col-md-3 col-xs-12 tree_left_asidebar emptabs_left_window">

        <div class="col-md-12 scroll_window">
            <div class="left_employee_window">

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_xox"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

                <div class="row">
                    <div class="col-md-12 col-xs-12 tab_box"></div>
                </div>

            </div>

        </div>
    </div>

    <div class="col-md-9 col-xs-12 employee_right_asidebar">

        <form role="form" method="post" action="">

            <div class="row employee_top_rightbar ">

                <div class="col-md-12 col-xs-12 employee_save_header">
                    @*<span style="font-size:14px;padding:10px; margin-top:20px; font-weight:bold;color:#000;">This is dummy content</span>*@

                    <button type="button" class="btn btn-default pull-right isee_button_color"><i class="fa fa-globe"> Show Map</i></button>
                    <button type="button" class="btn btn-default pull-right floppy-icon isee_button_color"><i class="fa fa-list-alt"> Set Details</i></button>
                    <button type="button" class="btn btn-default pull-right floppy-icon isee_button_color"><i class="fa fa-calendar"> Scheduler</i></button>
                    <button type="button" class="btn btn-default pull-right floppy-icon isee_button_color"><i class="fa fa-close"> Cancel</i></button>
                    <button type="button" class="btn btn-default pull-right floppy-icon isee_button_color"><i class="fa fa-save"> Save</i></button>

                </div>

                <div class="col-md-12 col-xs-12 employee_info_panel">
                    <div class="row">
                        <div class="col-md-6 col-xs-12 employee_left_form">
                            <div class="col-md-12 col-xs-12 left_inner_emptab">
                                <h4 class="customer_title">Employee Info</h4>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="usr">Number</label>
                                        <input class="form-control isee_input" id="" maxlength="50" name="Number" placeholder="0" name="number" type="tel" value="">
                                    </div>

                                    <div class="form-group">
                                        <label for="usr">First Name *</label>
                                        <input class="form-control isee_input" id="" name="firstname" type="text" value="" required>
                                    </div>

                                    <div class="form-group">
                                        <label for="usr">Start Day</label>
                                        <div id="datepicker1" class="input-group date" data-date-format="mm-dd-yyyy">
                                            <input class="form-control isee_input" type="text" id="" name="startday" value="" />
                                            <span class="input-group-addon"><i class="fa fa-th"></i></span></span>
                                        </div>
                                    </div>

                                </div>

                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="usr">Mail</label>
                                        <input class="form-control isee_input" id="" name="mail" type="email" value="">
                                    </div>

                                    <div class="form-group">
                                        <label for="usr">Last Name *</label>
                                        <input class="form-control isee_input" id="" name="lastname" type="text" value="" required>
                                    </div>


                                    <div class="form-group">
                                        <label for="usr">End Date</label>
                                        <div id="datepicker1" class="input-group date" data-date-format="mm-dd-yyyy">
                                            <input class="form-control isee_input" type="text" id="" title="enddate" value="" />
                                            <span class="input-group-addon"><i class="fa fa-th"></i></span></span>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>

                        <div class="col-md-6 col-xs-12 employee_right_window">
                            <h4 class="customer_title">Add An info</h4>
                            <div class="col-md-12 col-xs-12 right_inner_window">

                                <div class="col-md-6">
                                    <div class="row">
                                        <div class="col-xs-5">

                                            <div class="form-group">
                                                <label>Phone1 *</label>
                                                <input class="form-control isee_input" id="" maxlength="10" name="phone1" required="required" type="tel" value="" />
                                            </div>
                                        </div>

                                        <div class="col-xs-7">
                                            <label></label>
                                            <div class="form-group phone_no">
                                                <input class="form-control isee_input" id="" maxlength="50" name="phone" required="required" type="tel" value="" />
                                            </div>
                                        </div>
                                    </div>

                                    <div class="form-group">
                                        <label for="usr">Manufacture *</label>
                                        <select class="form-control tree_select_option isee_button_color" id="" maxlength="50" name="manufacture" value="" required>
                                            <option></option>
                                            <option>Samsung</option>
                                            <option>LG</option>
                                            <option>BlackBerry</option>
                                            <option>Apple</option>
                                            <option>Motorola</option>
                                            <option>Nokia</option>
                                        </select>
                                    </div>


                                </div>

                                <div class="col-md-6">

                                    <div class="row">
                                        <div class="col-xs-5">
                                            <div class="form-group">
                                                <label>Phone1 *</label>
                                                <input class="form-control isee_input" id="" maxlength="10" name="phone1" required="required" type="tel" value="" />
                                            </div>
                                        </div>
                                        <div class="col-xs-7">
                                            <label></label>
                                            <div class="form-group phone_no">
                                                <input class="form-control isee_input" id="" maxlength="50" name="phone" required="required" type="tel" value="" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label for="usr">Phone Type *</label>
                                        <select class="form-control tree_select_option isee_button_color" id="" name="phonetype" value="" required>
                                            <option></option>
                                            <option></option>
                                        </select>
                                    </div>

                                </div>

                                <div class="col-md-6">

                                    <div class="form-group">
                                        <label for="usr">Last application</label>
                                        <div id="datepicker1" class="input-group date" data-date-format="mm-dd-yyyy">
                                            <input class="form-control isee_input" type="text" id="" name="startday" value="" />
                                            <span class="input-group-addon"><i class="fa fa-th"></i></span></span>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <button type="button" class="btn btn-default pull-right send_application isee_button_color"><i class="fa fa-envelope"> Send Application</i></button>
                                </div>

                            </div>
                        </div>

                    </div>

                    <!----- ================End======================-->



                    <div class="container employee_navtab_window">

                        <div class="span12">
                            <ul class="nav nav-tabs">
                                <li class="active menu_tabs"><a href="#hello" data-toggle="tab"><i class="fa fa-mobile"> SMS</i></a></li>
                                <li class="menu_tabs"><a href="#empty" data-toggle="tab"><i class="fa fa-clock-o"> Time</i></a></li>
                                <li class="menu_tabs"><a href="#templates" data-toggle="tab"><i class="fa fa-group"> Tree</i></a></li>

                            </ul>

                            <div class="tab-content">
                                <div class="tab-pane" id="empty">
                                    <div class="container sms_tabs">
                                        <div class="col-md-6">
                                            <h4 class="customer_title">Fill Employee Hours</h4>
                                            <div class="tg-wrap_content_tabs">
                                                <table class="tg">
                                                    <tr>
                                                        <th class="tg-z1n2"></th>
                                                        <th class="tg-z1n2">Day Status</th>
                                                        <th class="tg-z1n2">Start Time</th>
                                                        <th class="tg-z1n2">End Time</th>
                                                        <th class="tg-z1n2">Start Time</th>
                                                        <th class="tg-z1n2">End Time</th>
                                                        <th class="tg-z1n2"></th>

                                                    </tr>



                                                </table>

                                            </div>
                                        </div>

                                        <div class="col-md-6">

                                            <div class="container right_content_tabs_window">
                                                <h4 class="customer_title">Select</h4>

                                                <div class="row">
                                                    <div class="col-sm-6">
                                                        <div class="form-group">
                                                            <label for="usr">Month</label>
                                                            <select class="form-control tree_select_option isee_button_color" id="" maxlength="50" name="manufacture" value="" required>
                                                                <option>January</option>
                                                                <option>Feburary</option>
                                                                <option>March</option>
                                                                <option>April</option>
                                                                <option>May</option>
                                                                <option>June</option>
                                                                <option>July</option>
                                                                <option>August</option>
                                                                <option>September</option>
                                                                <option>October</option>
                                                                <option>November</option>
                                                                <option>December</option>

                                                            </select>
                                                        </div>

                                                    </div>

                                                    <div class="col-sm-6">

                                                        <div class="form-group">
                                                            <label for="usr">Year</label>
                                                            <select class="form-control tree_select_option isee_button_color" id="" maxlength="50" name="manufacture" value="" required>
                                                                <option>2012</option>
                                                                <option>2013</option>
                                                                <option>2014</option>
                                                                <option>2015</option>
                                                                <option>2016</option>
                                                                <option>2017</option>
                                                                <option>2018</option>
                                                            </select>
                                                        </div>

                                                    </div>

                                                </div>

                                                <div class="row">
                                                    <div class="col-md-3 col-xs-3">
                                                    </div>

                                                    <div class="col-md-9 col-xs-9">
                                                        <button type="button" class="btn btn-default pull-right isee_button_color">Show Below</button>

                                                        <button type="button" class="btn btn-default full_screen pull-right isee_button_color">View Full Screen</button>
                                                    </div>

                                                </div>

                                                <div class="row">

                                                    <div class="col-md-12">
                                                        <div class="tg-wrap_content_tabs">
                                                            <table class="tg">
                                                                <tr>
                                                                    <th class="tg-z1n2"></th>
                                                                    <th class="tg-z1n2">Date</th>
                                                                    <th class="tg-z1n2">Status</th>
                                                                    <th class="tg-z1n2">SMS Description</th>
                                                                    <th class="tg-z1n2">Sent</th>
                                                                    <th class="tg-z1n2"></th>

                                                                </tr>



                                                            </table>

                                                        </div>

                                                    </div>
                                                </div>


                                            </div>

                                        </div>

                                    </div>
                                </div>

                                <div class="tab-pane active" id="hello">
                                    <div class="container sms_tabs">
                                        <div class="col-md-6">
                                            <h4 class="customer_title">Message</h4>
                                            <div class="col-md-12 left_content_area">

                                                <div class="col-md-12 main_content_area"></div>

                                                <button type="button" class="btn btn-default pull-right send_application isee_button_color"><i class="fa fa-mobile"> Send SMS</i></button>
                                            </div>
                                        </div>

                                        <div class="col-md-6">

                                            <div class="container right_content_tabs_window"></div>

                                            <div class="col-md-4"><h4 class="customer_title">Message History</h4></div>

                                            <div class="col-md-8">
                                                <div class="row">
                                                    <div class="col-sm-5">
                                                        <div class="form-group">
                                                            <label for="usr">From</label>
                                                            <div id="datepicker1" class="input-group date" data-date-format="mm-dd-yyyy">
                                                                <input class="form-control isee_input" type="text" id="" title="enddate" value="" />
                                                                <span class="input-group-addon"><i class="fa fa-th"></i></span></span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-sm-5">

                                                        <div class="form-group">
                                                            <label for="usr">To</label>
                                                            <div id="datepicker1" class="input-group date" data-date-format="mm-dd-yyyy">
                                                                <input class="form-control isee_input" type="text" id="" title="enddate" value="" />
                                                                <span class="input-group-addon"><i class="fa fa-th"></i></span></span>
                                                            </div>
                                                        </div>

                                                    </div>

                                                    <div class="col-sm-2">
                                                        <button type="button" class="btn btn-default pull-right send_application isee_button_color"><i class="fa fa-search"></i></button>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">

                                                <div class="col-md-12">
                                                    <div class="tg-wrap_content_tabs">
                                                        <table class="tg">
                                                            <tr>
                                                                <th class="tg-z1n2"></th>
                                                                <th class="tg-z1n2">Date</th>
                                                                <th class="tg-z1n2">Status</th>
                                                                <th class="tg-z1n2">SMS Description</th>
                                                                <th class="tg-z1n2">Sent</th>
                                                                <th class="tg-z1n2"></th>

                                                            </tr>



                                                        </table>

                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>


                                <div class="tab-pane" id="templates">
                                    <div class="container sms_tabs">
                                        <div class="col-md-6">
                                            <p class="text-left">Demo</p>
                                        </div>

                                        <div class="col-md-6">

                                            <div class="container right_content_tabs_window">
                                                <input class="form-control isee_input tree_input-tabs" id="" maxlength="50" name="phone2" type="text" value="" required />
                                            </div>

                                        </div>
                                    </div>
                                </div>

                            </div><!--.tab-content -->

                        </div><!--.span12 -->

                    </div><!--.container -->
                    <!------------------=====================================------>
                    @*<div class="row">
                        <div class="col-md-6 col-xs-12 employee_hours">
                                <div class="col-md-12 col-xs-12 employee_fill_left"><!--- change col-md-6 into col-md-12-->
                                     <h4 class="customer_title">Fill Employee Hours</h4>

                                    <div class="tg-wrap">
                                            <table class="tg">
                                                <tr>

                                                    <th class="tg-z1n2">Day Status</th>
                                                    <th class="tg-z1n2">Start Time</th>
                                                    <th class="tg-z1n2">End Time</th>
                                                    <th class="tg-z1n2">Start Time</th>
                                                    <th class="tg-z1n2">End Time</th>

                                                </tr>

                                                <tr>

                                                    <td class="tg-dx8v">Sun</td>
                                                    <td class="tg-dx8v">9:00 AM</td>
                                                    <td class="tg-dx8v">6:00 PM</td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>

                                                </tr>
                                                <tr>

                                                    <td class="tg-dx8v">Mon</td>
                                                    <td class="tg-dx8v">9:00 AM</td>
                                                    <td class="tg-dx8v">6:00 PM</td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>

                                                </tr>
                                                <tr>

                                                    <td class="tg-dx8v">Tue</td>
                                                    <td class="tg-dx8v">9:00 AM</td>
                                                    <td class="tg-dx8v">6:00 PM</td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>

                                                </tr>
                                                <tr>

                                                    <td class="tg-dx8v">Wed</td>
                                                    <td class="tg-dx8v">9:00 AM</td>
                                                    <td class="tg-dx8v">6:00 PM</td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>

                                                </tr>
                                                <tr>

                                                    <td class="tg-dx8v">Thu</td>
                                                    <td class="tg-dx8v">9:00 AM</td>
                                                    <td class="tg-dx8v">6:00 PM</td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>

                                                </tr>
                                                <tr>

                                                    <td class="tg-dx8v">Fri</td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>

                                                </tr>
                                                <tr>

                                                    <td class="tg-dx8v">Sat</td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>
                                                    <td class="tg-dx8v"></td>

                                                </tr>

                                            </table>
                                       </div>
                                    <h4 class="customer_title">Drag Item To Tree</h4>
                                    <div class="col-md-12 col-xs-12">
                                        <input class="form-control isee_input" id="" name="item"  value="">
                                    </div><!--- add new  col-md-12 for drag item -->

                                 </div>
                        </div>


                            <div class="col-md-6 col-xs-12 employee_inner_demo">
                                 <div class="col-md-12 col-xs-12 employee_inner_demo_first"><!--- 3 nov change col-md-6 into col-md-12-->

                                       <div class="col-md-6 col-xs-12 employee_inner_demo_second">
                                    <p>Demo Company</p>
                                        </div>
                                    </div>

                            </div>
                        </div>*@

                </div>
            </div>
            <!-- Customer right panel end---->
        </form>

    </div>


</div>

</form>










@section scripts
{

   <script type="text/javascript">
       $(function{
    $('#myTab a').click(function(e) {
        e.preventDefault();
        $(this).tab('show');
            });
            });
</script>
    
}