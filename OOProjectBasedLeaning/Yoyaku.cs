namespace OOProjectBasedLeaning
{

    public partial class Yoyaku : Form
    {

        public Yoyaku()
        {

            InitializeComponent();

            // �Q�X�g�̍쐬
            new GuestCreatorForm().Show();

            // ��
            new HomeForm().Show();

            // �z�e��
            new HotelForm().Show();

        }

    }

}
