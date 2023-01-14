using eegs_back_end.Admin.Sequencer.Model;
using eegs_back_end.DbModule;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.Form.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.Sequencer.Repository
{
    public interface ISequenceRepository : IGlobalInterface
    {
        List<FormModel> GetFormList(string domain_id);
        bool Save(SequenceModel model);

    }
    public class SequenceRepository : ISequenceRepository
    {
        public List<FormModel> GetFormList(string domain_id)
        {

            string sql = @"select distinct forms.form_guid from general.tbl_user_form forms

                           where forms.form_type = 'Form' and forms.domain_guid = '"+ domain_id +"'";

            List<string> form_id_list = (List<string>)QueryModule.DataSource<string>(sql);

            string form_guids = ObjectSqlMapping.SetParam(form_id_list.ToArray());

            //sql = @"select distinct general.tbl_user_form.form_guid 
            //        from general.tbl_user_form 
            //        where general.tbl_user_form.parent_guid = ''
            //        AND general.tbl_user_form.domain_guid = @domain_guid";
            sql = @"select distinct general.tbl_user_form.parent_guid 
                    from general.tbl_user_form 
                    where  tbl_user_form.form_guid IN (" + form_guids + ")";
            List<string> parent_guid_list = (List<string>)QueryModule.DataSource<string>(sql);



            string parent_guids = ObjectSqlMapping.SetParam(parent_guid_list.ToArray());

            sql = @"select distinct general.tbl_user_form.parent_guid 
                    from general.tbl_user_form 
                    where  tbl_user_form.form_guid IN (" + parent_guids + ")";
            List<string> super_parent_guids = (List<string>)QueryModule.DataSource<string>(sql);

            string super_parent_guids_ = ObjectSqlMapping.SetParam(super_parent_guids.ToArray());

            string merged_guids = form_guids + ", " + parent_guids + ", " + super_parent_guids_;

            sql = @"select general.tbl_user_form.* from general.tbl_user_form
                    where general.tbl_user_form.form_guid in (" + merged_guids + ")";
            List<FormModel> forms = (List<FormModel>)QueryModule.DataSource<FormModel>(sql);


            List<int> removeIndexs = new List<int>();
            //object[] Route = new object[activityTypes.Count];
            if (forms != null && forms.Count > 0)
            {
                int cnt = 0;
                int k = 0;
                foreach (var z in forms)
                {
                    if (z.parent_guid != null && z.parent_guid != "")
                    {
                        int index = forms.FindIndex(x => x.form_guid == z.parent_guid);

                        if (index > -1)
                        {
                            if (forms[index].child == null)
                            {
                                forms[index].child = new List<FormModel>();
                            }

                            //z.routes = activityTypes.AsEnumerable().Where(r => r.form_guid == z.form_guid).ToList();
                            if(z.routes != null)
                            {
                                if (z.routes.Count == 0)
                                {
                                    z.routes = null;
                                }
                                else
                                {
                                    foreach (var x in z.routes)
                                    {
                                        //Route[k] = x.executable_path;
                                        k++;
                                    }
                                }
                            }
                          

                            forms[index].child.Add(z);

                            removeIndexs.Add(cnt);
                        }
                    }
                    cnt++;
                }

            }
            for (int i = removeIndexs.Count - 1; i >= 0; i--)
            {
                forms.RemoveAt(removeIndexs[i]);
            }

            // REMOVE node that dont have child
            for (int i = forms.Count - 1; i >= 0; i--)
            {
                if (forms[i].child == null && forms[i].parent_guid != "")
                {
                    forms.RemoveAt(i);
                }
            }


            return forms;
        }

        public bool Save(SequenceModel model)
        {
            string user_id = GlobalObject.user_id;
            
            string sql = "delete from general.form_sort where user_id = '" + user_id + "'";
            QueryModule.Execute<int>(sql);

            foreach (Sequence seq in model.seq_list)
            {
                seq.user_id = user_id;
                 sql = "insert into general.form_sort (" + ObjectSqlMapping.MapInsert<Sequence>() + ")";
                int res = (int)QueryModule.Execute<int>(sql, seq);
            }

            return true;
        }
    }
}
