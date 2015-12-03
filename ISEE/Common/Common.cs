using ISEEDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISEE.Common
{
    public class Common
    {
        static ISEEEntities dataContext = new ISEEEntities();

        public static int GetInteger(string val)
        {

            int output;
            int.TryParse(val, out output);
            //if (isNaN(output))
            //    return output;
            return output;
        }
        public static string GetNullableValues(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return value;

        }
        public static TimeSpan? GetTimeSpan(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return Convert.ToDateTime(value) - Convert.ToDateTime("0:00");

        }
        public static DateTime? ConvertDateTime(string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;
            return new DateTime(Convert.ToInt32(date.Split('/')[2]), Convert.ToInt32(date.Split('/')[1]), Convert.ToInt32(date.Split('/')[0]));
        }
        public static DateTime ConvertDateTimeN(string date)
        {
            return new DateTime(Convert.ToInt32(date.Split('/')[2]), Convert.ToInt32(date.Split('/')[1]), Convert.ToInt32(date.Split('/')[0]));
        }
        public static List<TreeNodeData> CreateJsonTree(List<TreeView> data)
        {
            List<TreeNodeData> treeList = new List<TreeNodeData>();
            if (data.Count == 0)
            {
                treeList.Add(new TreeNodeData() { id = -100, text = SessionManagement.FactoryDesc, textCss = "customnode", objecttype = "companyNode" });
            }
            TreeNodeData parentTreeNode = new TreeNodeData();
            CreateTreeNodes(data, ref treeList, ref parentTreeNode, false);

            return treeList;
        }

        public static void CreateTreeNodes(List<TreeView> data, ref List<TreeNodeData> treeList, ref TreeNodeData parentTreeNode, bool hasChildren = false)
        {
            TreeNodeData objTreeNodeData;
            foreach (var objTreeView in data)
            {
                if (objTreeView.EmployeeID != null)
                {
                    var emp = dataContext.Employees.Where(x => x.EmployeeId == objTreeView.EmployeeID).FirstOrDefault();
                    objTreeNodeData = new TreeNodeData() { id = objTreeView.ID, text = emp.LastName + " " + emp.FirstName, objectid = objTreeView.EmployeeID, textCss = "employeeTitle", objecttype = "employee", iconUrl = "/images/img/employee_16.png" };
                }
                else if (objTreeView.CustomerID != null)
                {
                    var cust = dataContext.Customers.Where(x => x.CustomerId == objTreeView.CustomerID).FirstOrDefault();
                    objTreeNodeData = new TreeNodeData() { id = objTreeView.ID, text = cust.LastName + " " + cust.FirstName, objectid = objTreeView.CustomerID, textCss = "customerTitle", objecttype = "customer", iconUrl = "/images/img/customer_16.png" };
                }
                else
                    objTreeNodeData = new TreeNodeData() { id = objTreeView.ID, text = objTreeView.Description, textCss = "customnode", objecttype = objTreeView.ParentID == null ? "companyNode" : "branchNode" };
                if (hasChildren)
                {
                    if (parentTreeNode.children == null)
                    {
                        parentTreeNode.children = new List<TreeNodeData>();
                    }
                    parentTreeNode.children.Add(objTreeNodeData);
                }
                else
                {
                    treeList.Add(objTreeNodeData);
                }

                if (objTreeView.TreeView1.Any())
                {
                    CreateTreeNodes(objTreeView.TreeView1.ToList(), ref  treeList, ref objTreeNodeData, true);
                }
            }
        }

        private void DeleteNodes(List<TreeNodeData> treeNodeList, int factoryId)
        {
            List<long> _idsList = new List<long>();
            GetTreeIds(treeNodeList, ref _idsList);
            var deleteNodes = dataContext.TreeViews.Where(c => !_idsList.Contains(c.ID) && c.ID > 0 && c.FactoryID == factoryId).ToList();
            for (int i = deleteNodes.Count; i > 0; i--)
            {
                dataContext.TreeViews.Remove(deleteNodes[i - 1]);
            }
            dataContext.SaveChanges();
        }

        private void GetTreeIds(List<TreeNodeData> treeNodeList, ref  List<long> _idsList)
        {
            foreach (var item in treeNodeList)
            {
                _idsList.Add(item.id);
                if (item.children != null && item.children.Any())
                {
                    GetTreeIds(item.children, ref _idsList);
                }
            }
        }
        public enum WeekDays
        {
            Sunday = 1,
            Monday = 2,
            Tuesday = 3,
            Wednesday = 4,
            Thursday = 5,
            Friday = 6,
            Saturday = 7

        }
    }
}