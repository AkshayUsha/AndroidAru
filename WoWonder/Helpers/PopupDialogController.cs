using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Text.Format;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using WoWonder.Activities.Default;
using Client = WoWonder_API.Requests.Client;
using Exception = System.Exception;

namespace WoWonder.Helpers
{
   public class PopupDialogController : Java.Lang.Object, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, MaterialDialog.IInputCallback
    {
        public Activity ActivityContext;
        public string TypeDialog;
        public PopupDialogController(Activity Activtycontext,string typeDialog)
        {
            ActivityContext = Activtycontext;
            TypeDialog = typeDialog;
        }

       public async void ShowPlayListDialog()
       {
           try
           {
               List<string> arrayAdapter = new List<string>();
               MaterialDialog.Builder DialogList = new MaterialDialog.Builder(ActivityContext);

               //var count = Classes.PlayListsVideosList.Count;
               //if (count > 0)
               //{
               //    for (int i = 0; i < count; i++)
               //    {
               //        //if (!string.IsNullOrEmpty(Classes.PlayListsVideosList[i].dp_name))
               //        //    arrayAdapter.Add(Classes.PlayListsVideosList[i].dp_name);
               //    }
               //}

               //DialogList.Title(ActivityContext.GetText(Resource.String.Lbl_Select_One_Name));
               //DialogList.Items(arrayAdapter);
               //DialogList.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Creat_new)).OnPositive(this);
               //DialogList.NegativeText(ActivityContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
               //DialogList.ItemsCallback(this).Build().Show();
            }
           catch (Exception exception)
           {
               Crashes.TrackError(exception);
           }
       }

        public void ShowNormalDialog(string title, string content =null, string positiveText =null, string negativeText = null)
        {
            try
            {
                MaterialDialog.Builder DialogList = new MaterialDialog.Builder(ActivityContext);
               
                if (!string.IsNullOrEmpty(title))
                    DialogList.Title(title);

                if (!string.IsNullOrEmpty(content))
                    DialogList.Content(content);

                if (!string.IsNullOrEmpty(negativeText))
                {
                    DialogList.NegativeText(negativeText);
                    DialogList.OnNegative(this);
                }

                if (!string.IsNullOrEmpty(positiveText))
                {
                    DialogList.PositiveText(positiveText);
                    DialogList.OnPositive(this);
                }

                DialogList.Build().Show();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void ShowEditTextDialog(string title, string content = null, string positiveText = null, string negativeText = null)
        {
            try
            {
                MaterialDialog.Builder DialogList = new MaterialDialog.Builder(ActivityContext);

                if (!string.IsNullOrEmpty(title))
                    DialogList.Title(title);

                if (!string.IsNullOrEmpty(content))
                    DialogList.Content(content);

                if (!string.IsNullOrEmpty(negativeText))
                {
                    DialogList.NegativeText(negativeText);
                    DialogList.OnNegative(this);
                }

                if (!string.IsNullOrEmpty(positiveText))
                {
                    DialogList.PositiveText(positiveText);
                    DialogList.OnPositive(this);
                }
                
                DialogList.InputType(InputTypes.ClassText | InputTypes.TextFlagMultiLine);
                DialogList.Input("", "", this);
                DialogList.Build().Show();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }


        public void OnSelection(MaterialDialog p0, View p1, int p2, ICharSequence SelectedPlayListName)
        {
            
        }

                 
        public async void OnClick(MaterialDialog p0, DialogAction p1)
        {
            if (TypeDialog == "PlayList")
            {
                if (p1 == DialogAction.Positive)
                {
                  
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }  
            else if (TypeDialog =="Login")
            {
                if (p1 == DialogAction.Positive)
                {
                    ActivityContext.StartActivity(new Intent(ActivityContext, typeof(Login_Activity)));
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                } 
            }
            else if (TypeDialog == "DeleteAcount")
            {
                if (p1 == DialogAction.Positive)
                {
                   // ActivityContext.StartActivity(new Intent(ActivityContext, typeof(DeleteAcount_Activity)));
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                } 
            }
            else if (TypeDialog == "Logout")
            {
                if (p1 == DialogAction.Positive)
                {
                    await RemoveData("Logout");

                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                   var ss = await dbDatabase.CheckTablesStatus();
                    dbDatabase.Dispose(); 

                    Intent intent = new Intent(ActivityContext, typeof(First_Activity));
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetAction(Intent.ActionMain);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    ActivityContext.StartActivity(intent);
                    ActivityContext.FinishAffinity();
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                } 
            }
        }


        public async Task RemoveData(string type)
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                 dbDatabase.DropAll();
                 dbDatabase.ClearAll();
                dbDatabase.Dispose();

                Classes.ClearAllList();

                if (type == "Logout")
                {
                    IMethods.IPath.DeleteAll_MyFolderDisk();

                    if (IMethods.CheckConnectivity())
                    {
                        var data = await Client.Global.Get_Delete_Token().ConfigureAwait(false);
                    }
                }
                else
                {
                    IMethods.IPath.DeleteAll_MyFolder();
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                if (TypeDialog == "Report")
                {
                    if (p1.Length() > 0)
                    {
                        if (IMethods.CheckConnectivity())
                        {
                       
                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_received_your_report), ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection) , ToastLength.Short).Show();
                        }
                    }
                    else 
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_The_name_can_not_be_blank), ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }
    }

    public class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
    {
        public static readonly string TAG = "MyTimePickerFragment";
        Action<DateTime> timeSelectedHandler = delegate { };

        public static TimePickerFragment NewInstance(Action<DateTime> onTimeSelected)
        {
            TimePickerFragment frag = new TimePickerFragment();
            frag.timeSelectedHandler = onTimeSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currentTime = DateTime.Now;
            bool is24HourFormat = DateFormat.Is24HourFormat(Activity);
            TimePickerDialog dialog = new TimePickerDialog (Activity, Resource.Style.MyTimePickerDialogTheme, this, currentTime.Hour, currentTime.Minute, is24HourFormat);
            return dialog;
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            DateTime currentTime = DateTime.Now;
            DateTime selectedTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hourOfDay, minute, 0);
           
            timeSelectedHandler(selectedTime);
        }
    }

    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> _dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            DatePickerDialog dialog = new DatePickerDialog(Activity, Resource.Style.MyTimePickerDialogTheme,this,currently.Year,currently.Month - 1,currently.Day);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            _dateSelectedHandler(selectedDate);
        }
    }
}