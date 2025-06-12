namespace OOProjectBasedLeaning
{

    public partial class Yoyaku : Form
    {

        public Yoyaku()
        {

            InitializeComponent();

            // ゲストの作成
            new GuestCreatorForm().Show();

            // 家
            new HomeForm().Show();

            // ホテル
            new HotelForm().Show();

        }

    }

}
