using ClinicCRM.Models;
using ClinicCRM.Services;

namespace ClinicCRM;

public sealed class MainForm : Form
{
    private readonly ClinicService _service = new();
    private readonly TabControl _tabs = new() { Dock=DockStyle.Fill };
    private readonly List<MedicalEmployee> _employees = new();
    private readonly DataGridView _doctorsGrid = Grid();
    private readonly DataGridView _dtoGrid = Grid();
    private readonly DataGridView _groupGrid = Grid();
    private readonly ComboBox _doctorChoice = new() { DropDownStyle=ComboBoxStyle.DropDownList, Width=260 };
    private readonly ComboBox _category = new() { DropDownStyle=ComboBoxStyle.DropDownList, Width=140 };
    private readonly ComboBox _departmentFilter = new() { DropDownStyle=ComboBoxStyle.DropDownList, Width=170 };
    private readonly TextBox _patientSearch = new() { Width=160, PlaceholderText="Поиск пациента" };
    private readonly CheckBox _completedOnly = new() { Text="Только завершенные", AutoSize=true };
    private readonly Label _revenue = new() { AutoSize=true, Font=new Font("Segoe UI",10,FontStyle.Bold) };
    private readonly ListBox _eventLog = new() { Dock=DockStyle.Fill };
    private List<AppointmentViewDto> _allDtos = new();
    private int _nextLocalId = 100;

    public MainForm()
    {
        Text="Clinic CRM — информационная система медицинской клиники"; Width=1150; Height=760; StartPosition=FormStartPosition.CenterScreen;
        _category.Items.AddRange(new object[]{"Высшая","Первая"}); _category.SelectedIndex=0;
        _service.AppointmentRegistered += (_,e) => _eventLog.Items.Insert(0,$"{e.Message}: {e.PatientName}, врач {e.DoctorName}, {e.Cost:N0} тг");
        _service.Logger = message => { if(IsHandleCreated) BeginInvoke(new Action(()=>Text="Clinic CRM — "+message)); };
        BuildDoctorsTab(); BuildAppointmentTab(); BuildAnalyticsTab(); BuildGroupsTab();
        Controls.Add(_tabs);
        ReloadAll();
    }

    private static DataGridView Grid() => new() { Dock=DockStyle.Fill, ReadOnly=true, AllowUserToAddRows=false, AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill, SelectionMode=DataGridViewSelectionMode.FullRowSelect, MultiSelect=false };
    private static Button Btn(string text) => new() { Text=text, AutoSize=true, Padding=new Padding(7,4,7,4) };
    private static FlowLayoutPanel Bar() => new() { Dock=DockStyle.Top, AutoSize=true, Padding=new Padding(8), WrapContents=true };
    private void AddTab(string name, Control content) { var page=new TabPage(name); page.Controls.Add(content); _tabs.TabPages.Add(page); }

    private void BuildDoctorsTab()
    {
        var panel=new Panel { Dock=DockStyle.Fill }; var bar=Bar();
        var name=new TextBox { Width=210, PlaceholderText="ФИО врача" }; var salary=new NumericUpDown { Width=120, Maximum=10000000, Minimum=1000, Value=100000, Increment=5000, ThousandsSeparator=true };
        var dep=new ComboBox { Width=150, DropDownStyle=ComboBoxStyle.DropDownList, Name="doctorDepartment" };
        bar.Controls.AddRange(new Control[]{new Label{Text="ФИО:",AutoSize=true,Padding=new Padding(0,8,0,0)},name,new Label{Text="Оклад:",AutoSize=true,Padding=new Padding(0,8,0,0)},salary,_category,dep});
        var add=Btn("Добавить врача"); add.Click+=(_,_)=>{
            if(string.IsNullOrWhiteSpace(name.Text)||dep.SelectedItem is not DepartmentItem d){MessageBox.Show("Введите ФИО и выберите отделение.");return;}
            var doc=new Doctor(_nextLocalId++,name.Text.Trim(),salary.Value,_category.Text,d.Id);
            _employees.Add(doc); _service.GetDoctors();
            using var c=Database.OpenConnection(); using var cmd=c.CreateCommand(); cmd.CommandText="INSERT INTO Doctors(FullName,DepartmentId,Category,BaseSalary) VALUES($n,$d,$cat,$s)";
            cmd.Parameters.AddWithValue("$n",doc.FullName);cmd.Parameters.AddWithValue("$d",doc.DepartmentId);cmd.Parameters.AddWithValue("$cat",doc.Category);cmd.Parameters.AddWithValue("$s",doc.BaseSalary);cmd.ExecuteNonQuery();
            name.Clear(); ReloadAll();
        };
        var sort=Btn("Сортировать по окладу (IComparable)"); sort.Click+=(_,_)=>{_employees.Sort();_doctorsGrid.DataSource=null;_doctorsGrid.DataSource=_employees.Select(e=>new {e.Id,e.FullName,e.BaseSalary,Тип=e.GetType().Name,Доход=e.CalculateMonthlyPayout(10)}).ToList();};
        var clone=Btn("Клонировать (ICloneable)"); clone.Click+=(_,_)=>{
            var original=_employees.OfType<Doctor>().FirstOrDefault(); if(original==null){MessageBox.Show("Нет врача для клонирования.");return;}
            var copy=(Doctor)original.Clone(); copy.License.Number+="-COPY"; MessageBox.Show($"Глубокая копия создана.\nОригинал: {original.License.Number}\nКлон: {copy.License.Number}\nЛицензии — разные объекты: {!ReferenceEquals(original.License,copy.License)}");
        };
        var export=Btn("Экспорт отчета (явная реализация)"); export.Click+=(_,_)=>{var d=_employees.OfType<Doctor>().FirstOrDefault();if(d==null){MessageBox.Show("Нет врача.");return;} MessageBox.Show(((IExportable)d).GetSummary());};
        bar.Controls.AddRange(new Control[]{add,sort,clone,export});
        panel.Controls.Add(_doctorsGrid);panel.Controls.Add(bar); AddTab("1. Врачи и ООП-модель",panel);
        _departmentFilter.Tag=dep;
    }

    private sealed class DepartmentItem { public int Id {get;} public string Title {get;} public DepartmentItem(int id,string title){Id=id;Title=title;} public override string ToString()=>Title; }

    private void BuildAppointmentTab()
    {
        var panel=new Panel {Dock=DockStyle.Fill};var bar=Bar();var patient=new TextBox{Width=200,PlaceholderText="ФИО пациента"};
        var cost=new NumericUpDown{Width=120,Minimum=100,Maximum=10000000,Value=3500,Increment=100,ThousandsSeparator=true};var date=new DateTimePicker{Width=130,Format=DateTimePickerFormat.Short};
        bar.Controls.AddRange(new Control[]{new Label{Text="Врач:",AutoSize=true,Padding=new Padding(0,8,0,0)},_doctorChoice,new Label{Text="Пациент:",AutoSize=true,Padding=new Padding(0,8,0,0)},patient,new Label{Text="Стоимость:",AutoSize=true,Padding=new Padding(0,8,0,0)},cost,date});
        var register=Btn("Зарегистрировать прием");register.Click+=(_,_)=>{if(_doctorChoice.SelectedItem is not DoctorItem d||string.IsNullOrWhiteSpace(patient.Text)){MessageBox.Show("Выберите врача и укажите пациента.");return;} _service.RegisterAppointment(d.Id,patient.Text.Trim(),cost.Value,date.Value);patient.Clear();ReloadAll();};bar.Controls.Add(register);
        panel.Controls.Add(_eventLog);panel.Controls.Add(bar);AddTab("2. Запись на прием и Events",panel);
    }

    private sealed class DoctorItem { public int Id{get;} public string Name{get;} public DoctorItem(int id,string name){Id=id;Name=name;}public override string ToString()=>Name; }

    private void BuildAnalyticsTab()
    {
        var panel=new Panel{Dock=DockStyle.Fill};var bar=Bar();bar.Controls.AddRange(new Control[]{_departmentFilter,_patientSearch,_completedOnly,_revenue});
        _departmentFilter.SelectedIndexChanged+=(_,_)=>ApplyFilters();_patientSearch.TextChanged+=(_,_)=>ApplyFilters();_completedOnly.CheckedChanged+=(_,_)=>ApplyFilters();
        panel.Controls.Add(_dtoGrid);panel.Controls.Add(bar);AddTab("3. LINQ-Аналитика и DTO",panel);
    }

    private void BuildGroupsTab()
    {
        var panel=new Panel{Dock=DockStyle.Fill};var bar=Bar();var aggregates=Btn("Выручка по врачам (GroupBy)");aggregates.Click+=(_,_)=>{
            var doctors=_service.GetDoctors();var data=_service.GetAppointments().GroupBy(a=>a.DoctorId).Select(g=>new{Врач=doctors.FirstOrDefault(d=>d.Id==g.Key)?.FullName??"Неизвестно",КоличествоПриемов=g.Count(),СуммарныйДоход=g.Sum(a=>a.Cost),СреднийЧек=g.Average(a=>a.Cost)}).ToList();_groupGrid.DataSource=data;
        };
        var intersect=Btn("Пересечение пациентов (Intersect)");intersect.Click+=(_,_)=>{var sets=_service.GetPatientSets();_groupGrid.DataSource=sets.Both.Select(x=>new{Пациент=x,Результат="Терапия и Хирургия"}).ToList();};
        var except=Btn("Исключение (Except)");except.Click+=(_,_)=>{var sets=_service.GetPatientSets();_groupGrid.DataSource=sets.TherapyOnly.Select(x=>new{Пациент=x,Результат="Только Терапия"}).ToList();};
        var page=Btn("Показать страницу 1 (Skip/Take)");page.Click+=(_,_)=>_groupGrid.DataSource=_service.GetPage(1,5);
        bar.Controls.AddRange(new Control[]{aggregates,intersect,except,page});panel.Controls.Add(_groupGrid);panel.Controls.Add(bar);AddTab("4. Группировка и множества",panel);
    }

    private void ReloadAll()
    {
        var departments=_service.GetDepartments();
        var depControl=_departmentFilter.Tag as ComboBox;
        if(depControl!=null){depControl.Items.Clear();foreach(var d in departments)depControl.Items.Add(new DepartmentItem(d.Id,d.Title));if(depControl.Items.Count>0)depControl.SelectedIndex=0;}
        _departmentFilter.Items.Clear();_departmentFilter.Items.Add("Все отделения");foreach(var d in departments)_departmentFilter.Items.Add(d.Title);_departmentFilter.SelectedIndex=0;
        _doctorChoice.Items.Clear();foreach(var d in _service.GetDoctors())_doctorChoice.Items.Add(new DoctorItem(d.Id,d.FullName));if(_doctorChoice.Items.Count>0)_doctorChoice.SelectedIndex=0;
        _employees.Clear();foreach(var d in _service.GetDoctors())_employees.Add(d);
        _doctorsGrid.DataSource=_employees.Select(e=>new{e.Id,e.FullName,e.BaseSalary,Тип=e.GetType().Name,Доход=e.CalculateMonthlyPayout(_service.GetAppointments().Count(a=>a.DoctorId==e.Id))}).ToList();
        _allDtos=_service.GetAppointmentDtos();ApplyFilters();
    }

    private void ApplyFilters()
    {
        if(_allDtos==null)return;
        var query=_allDtos.AsEnumerable();
        if(_departmentFilter.SelectedItem is string dep&&dep!="Все отделения")query=query.Where(x=>x.Department==dep);
        if(!string.IsNullOrWhiteSpace(_patientSearch.Text))query=query.Where(x=>x.Patient.Contains(_patientSearch.Text,StringComparison.OrdinalIgnoreCase));
        if(_completedOnly.Checked)query=query.Where(x=>x.Status=="Completed");
        var result=query.ToList();_dtoGrid.DataSource=result;_revenue.Text=$"Итоговая выручка: {result.Sum(x=>x.Cost):N0} тг | записей: {result.Count}";
    }
}
