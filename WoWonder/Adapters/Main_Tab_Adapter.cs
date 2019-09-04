using System.Collections.Generic;
using Android.Support.V4.App;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Exception = System.Exception;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;


namespace WoWonder.Adapters
{
    public class Main_Tab_Adapter : FragmentStatePagerAdapter
    {
        public Main_Tab_Adapter(SupportFragmentManager sfm) : base(sfm)
        {
            try
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public List<SupportFragment> Fragments { get; set; }
        public List<string> FragmentNames { get; set; }

        public override int Count => Fragments.Count;

        public void AddFragment(SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void ClaerFragment()
        {
            try
            {
                Fragments.Clear();
                FragmentNames.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void RemoveFragment(SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Remove(fragment);
                FragmentNames.Remove(name);
                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public void InsertFragment(int index, SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Insert(index, fragment);
                FragmentNames.Insert(index, name);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
        }

        public override SupportFragment GetItem(int position)
        {
            try
            {
                if (Fragments[position] != null)
                    return Fragments[position];
                return Fragments[0];
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
                return null;
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(FragmentNames[position]);
        }
    }
}