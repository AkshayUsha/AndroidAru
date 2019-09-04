using System;
using Android.Content;
using Android.Content.Res;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Com.Facebook.Drawee.View;
using Microsoft.AppCenter.Crashes;
using Object = Java.Lang.Object;

namespace WoWonder.Helpers
{
    public  class AvatarImageBehavior :CoordinatorLayout.Behavior
    {
        private  static float MIN_AVATAR_PERCENTAGE_SIZE = 0.3f;
        private  static int EXTRA_FINAL_AVATAR_PADDING = 80;

        private  static String TAG = "behavior";
        private Context mContext;

        private float mCustomFinalYPosition;
        private float mCustomStartXPosition;
        private float mCustomStartToolbarPosition;
        private float mCustomStartHeight;
        private float mCustomFinalHeight;

        private float mAvatarMaxSize;
        private float mFinalLeftAvatarPadding;
        private float mStartPosition;
        private int mStartXPosition;
        private float mStartToolbarPosition;
        private int mStartYPosition;
        private int mFinalYPosition;
        private int mStartHeight;
        private int mFinalXPosition;
        private float mChangeBehaviorPoint;


        public AvatarImageBehavior(Context context, IAttributeSet attrs)
        {
            try
            {
                mContext = context;

                if (attrs != null)
                {
                    TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.AvatarImageBehavior);
                    mCustomFinalYPosition = a.GetDimension(Resource.Styleable.AvatarImageBehavior_finalYPosition, 0);
                    mCustomStartXPosition = a.GetDimension(Resource.Styleable.AvatarImageBehavior_startXPosition, 0);
                    mCustomStartToolbarPosition = a.GetDimension(Resource.Styleable.AvatarImageBehavior_startToolbarPosition, 0);
                    mCustomStartHeight = a.GetDimension(Resource.Styleable.AvatarImageBehavior_startHeight, 0);
                    mCustomFinalHeight = a.GetDimension(Resource.Styleable.AvatarImageBehavior_finalHeight, 0);

                    a.Recycle();
                }

                init();

                mFinalLeftAvatarPadding = context.Resources.GetDimension(2);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
               
            }
            
        }

        private void init()
        {
            bindDimensions();
        }

        private void bindDimensions()
        {
            mAvatarMaxSize = mContext.Resources.GetDimension(75);
        }

        public override bool LayoutDependsOn(CoordinatorLayout parent, Object child, View dependency)
        {
            return base.LayoutDependsOn(parent, child, dependency);
        }

        public override bool OnDependentViewChanged(CoordinatorLayout parent, Object child, View dependency)
        {
            var Style = (SimpleDraweeView) child;
            maybeInitProperties(Style, dependency);

            int maxScrollDistance = (int)(mStartToolbarPosition);
            float expandedPercentageFactor = dependency.GetY() / maxScrollDistance;
            if (expandedPercentageFactor < mChangeBehaviorPoint)
            {
                float heightFactor = (mChangeBehaviorPoint - expandedPercentageFactor) / mChangeBehaviorPoint;

                float distanceXToSubtract = ((mStartXPosition - mFinalXPosition)
                                             * heightFactor) + (Style.Height / 2);
                float distanceYToSubtract = ((mStartYPosition - mFinalYPosition)
                                             * (1f - expandedPercentageFactor)) + (Style.Height / 2);

                Style.SetX(mStartXPosition - distanceXToSubtract);
                Style.SetY(mStartYPosition - distanceYToSubtract);

                float heightToSubtract = ((mStartHeight - mCustomFinalHeight) * heightFactor);

                CoordinatorLayout.LayoutParams lp = (CoordinatorLayout.LayoutParams)Style.LayoutParameters;
                lp.Width = (int)(mStartHeight - heightToSubtract);
                lp.Height = (int)(mStartHeight - heightToSubtract);
                Style.LayoutParameters = lp;
            }
            else
            {
                float distanceYToSubtract = ((mStartYPosition - mFinalYPosition)
                                             * (1f - expandedPercentageFactor)) + (mStartHeight / 2);

                Style.SetX(mStartXPosition - Style.Width / 2);
                Style.SetY(mStartYPosition - distanceYToSubtract);

                CoordinatorLayout.LayoutParams lp = (CoordinatorLayout.LayoutParams)Style.LayoutParameters;
                lp.Width = (int)(mStartHeight);
                lp.Height = (int)(mStartHeight);
                Style.LayoutParameters = lp;
            }

            return true;
           
        }


        private void maybeInitProperties(SimpleDraweeView child, View dependency)
        {
            if (mStartYPosition == 0)
                mStartYPosition = (int)(dependency.GetY());

            if (mFinalYPosition == 0)
                mFinalYPosition = (dependency.Height / 2);

            if (mStartHeight == 0)
                mStartHeight = child.Height;

            if (mStartXPosition == 0)
                mStartXPosition = (int)(child.GetX() + (child.Width / 2));

            if (mFinalXPosition == 0)
                mFinalXPosition = mContext.Resources.GetDimensionPixelOffset(50) + ((int)mCustomFinalHeight / 2);

            if (mStartToolbarPosition == 0)
                mStartToolbarPosition = dependency.GetY();

            if (mChangeBehaviorPoint == 0)
            {
                mChangeBehaviorPoint = (child.Height - mCustomFinalHeight) / (2f * (mStartYPosition - mFinalYPosition));
            }
        }

        public int getStatusBarHeight()
        {
            int result = 0;
            int resourceId = mContext.Resources.GetIdentifier("status_bar_height", "dimen", "android");

            if (resourceId > 0)
            {
                result = mContext.Resources.GetDimensionPixelSize(resourceId);
            }
            return result;
        }
    }


}