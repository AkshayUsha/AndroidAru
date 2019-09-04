using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Microsoft.AppCenter.Crashes;
using SettingsConnecter;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;


namespace WoWonder.Helpers
{
    public class CategoriesController
    {
        public static ObservableCollection<Classes.Catigories> ListCatigories_Names = new ObservableCollection<Classes.Catigories>();

        //Categories Communities Local Custom
        public string[] Categories_CreateComunities = Application.Context.Resources.GetStringArray(Resource.Array.Categories_Communities_array);

        public void Get_Categories_Communities()
        {
            try
            {
                if (Settings.CategoriesComunities_Local)
                {
                    //Get Categories Local
                    if (Categories_CreateComunities.Length > 0)
                    {
                        ListCatigories_Names.Clear();

                        for (int i = 1; i <= Categories_CreateComunities.Length; i++)
                        {
                            Classes.Catigories cat = new Classes.Catigories();

                            string id = i.ToString();
                            string name = Categories_CreateComunities[i].ToString();

                            cat.Catigories_Id = id;
                            cat.Catigories_Name = name;
                            cat.Catigories_Color = "#212121";

                            var select = ListCatigories_Names.FirstOrDefault(a => a.Catigories_Id == id);
                            if (select == null)
                            {
                                ListCatigories_Names.Add(cat);
                            }
                        }
                    }
                }
                else
                {
                    
                    if (ListCatigories_Names.Count == 0)
                    {
                        SqLiteDatabase sqLiteDatabase = new SqLiteDatabase();
                        ObservableCollection<Classes.Catigories> listCategories = sqLiteDatabase.Get_CategoriesList();
                        if (listCategories?.Count > 0)
                            ListCatigories_Names = listCategories;
                        else
                            GetCategories_API();

                        sqLiteDatabase.Dispose();
                    }
                    else
                    {
                        GetCategories_API();
                    }
                  
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Get_Categories_Communities();
            }
        }
         
        public async void GetCategories_API()
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    var (Api_status, Respond) = await Client.Global.Get_Categories(new Settings());
                    if (Api_status == 200)
                    {
                        if (Respond is Categories_Object result)
                        {
                            if (result.Config.PageCategories.Count > 0)
                            {
                                ListCatigories_Names.Clear();

                                foreach (var pageCategory in result.Config.PageCategories)
                                {
                                    var id = pageCategory.Key;
                                    var name = pageCategory.Value;
                                    Classes.Catigories cat = new Classes.Catigories
                                    {
                                        Catigories_Id = id, Catigories_Name = name, Catigories_Color = "#212121"
                                    };

                                    var select = ListCatigories_Names.FirstOrDefault(a => a.Catigories_Id == id);
                                    if (select == null)
                                    {
                                        ListCatigories_Names.Add(cat);
                                    }
                                }
                            }

                            var list = ListCatigories_Names.Select(cat => new DataTables.CatigoriesTB
                            {
                                Catigories_Id = cat.Catigories_Id,
                                Catigories_Name = cat.Catigories_Name,
                            }).ToList();

                            SqLiteDatabase sqLiteDatabase = new SqLiteDatabase();
                            //Insert Categories in Database
                            sqLiteDatabase.Insert_Categories(new ObservableCollection<DataTables.CatigoriesTB>(list));
                            sqLiteDatabase.Dispose();
                        }
                    }
                    else if (Api_status == 400)
                    {
                        if (Respond is Error_Object error)
                        {
                            var errortext = error._errors.Error_text;
                             

                            //if (errortext.Contains("Invalid or expired access_token"))
                            //    API_Request.Logout(this);
                        }
                    }
                    else if (Api_status == 404)
                    {
                        var error = Respond.ToString();
                        //Show a Error
                        
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);  
            }
        }
 
        public string Get_Translate_Categories_Communities(string idCategory , string textCategory)
        {
            try
            {
                if (Categories_CreateComunities.Length <= 0) return textCategory;

                if (string.IsNullOrEmpty(textCategory))
                    return Application.Context.GetText(Resource.String.Lbl_Unknown);
                else
                {
                    string categoryName = int.Parse(idCategory) - 1 >= 0 ? Categories_CreateComunities[int.Parse(idCategory) - 1] : textCategory;
                    return categoryName;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return textCategory;
            }
        }
    }
}