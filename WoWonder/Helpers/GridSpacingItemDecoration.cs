using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;


namespace WoWonder.Helpers
{
    public class GridSpacingItemDecoration : RecyclerView.ItemDecoration
    {
        private int spanCount;
        private int spacing;
        private bool includeEdge;

        public GridSpacingItemDecoration(int spanCount, int spacing, bool includeEdge)
        {
            this.spanCount = spanCount;
            this.spacing = spacing;
            this.includeEdge = includeEdge;
        }


        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            base.GetItemOffsets(outRect, view, parent, state);
            int position = parent.GetChildAdapterPosition(view);
            int column = position % spanCount;

            if (includeEdge)
            {
                outRect.Left = spacing - column * spacing / spanCount;
                outRect.Right = (column + 1) * spacing / spanCount;

                if (position < spanCount)
                {
                    outRect.Top = spacing;
                }
                outRect.Bottom = spacing;
            }
            else
            {
                outRect.Left = column * spacing / spanCount;
                outRect.Right = spacing - (column + 1) * spacing / spanCount;

                if (position >= spanCount)
                {
                    outRect.Top = spacing;
                }
            }
        }
    }
}