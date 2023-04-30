﻿using FoodOrdersView;
using FoodOrdersContracts.BindingModels;
using FoodOrdersContracts.BusinessLogicsContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FoodOrdersContracts.DI;

namespace FoodOrdersView
{
	public partial class FormViewImplementers : Form
	{
		private readonly ILogger _logger;
		private readonly IImplementerLogic _logic;
		public FormViewImplementers(ILogger<FormViewImplementers> logger, IImplementerLogic logic)
		{
			InitializeComponent();
			_logger = logger;
			_logic = logic;
		}
		private void FormViewImplementers_Load(object sender, EventArgs e)
		{
			LoadData();
		}
		private void LoadData()
		{
			try
			{
                dataGridView.FillAndConfigGrid(_logic.ReadList(null));
                _logger.LogInformation("Загрузка исполнителей");
            }
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка загрузки исполнителей");
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK,
				MessageBoxIcon.Error);
			}
		}
		private void ButtonAdd_Click(object sender, EventArgs e)
		{
            var form = DependencyManager.Instance.Resolve<FormImplementer>();
			if (form.ShowDialog() == DialogResult.OK)
			{
				LoadData();
			}
		}
		private void ButtonUpd_Click(object sender, EventArgs e)
		{
			if (dataGridView.SelectedRows.Count == 1)
			{
                var form = DependencyManager.Instance.Resolve<FormImplementer>();
				form.Id = Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);
				if (form.ShowDialog() == DialogResult.OK)
				{
					LoadData();
				}
			}
		}
		private void ButtonDel_Click(object sender, EventArgs e)
		{
			if (dataGridView.SelectedRows.Count == 1)
			{
				if (MessageBox.Show("Удалить запись?", "Вопрос",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					int id =
					Convert.ToInt32(dataGridView.SelectedRows[0].Cells["Id"].Value);
					_logger.LogInformation("Удаление исполнителя");
					try
					{
						if (!_logic.Delete(new ImplementerBindingModel
						{
							Id = id
						}))
						{
							throw new Exception("Ошибка при удалении. Дополнительная информация в логах.");
						}
						LoadData();
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Ошибка удаления исполнителя");
						MessageBox.Show(ex.Message, "Ошибка",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}
		private void ButtonRef_Click(object sender, EventArgs e)
		{
			LoadData();
		}
	}
}
