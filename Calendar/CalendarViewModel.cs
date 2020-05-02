using System;
using System.Collections.Generic;
using System.Globalization;

namespace Calendar.ViewModels
{
	public class CalendarViewModel : BaseViewModel
	{
		#region Years

		private int _minYear { get; set; } = DateTime.Now.Year;
		public int MinYear
		{
			get => _minYear;
			set
			{
				if (value != _minYear)
				{
					_minYear = value;
					RaisePropertyChanged();
				}
			}
		}

		private int _maxYear { get; set; } = DateTime.Now.Year + 10;
		public int MaxYear
		{
			get => _maxYear;
			set
			{
				if (_maxYear != value)
				{
					_maxYear = value;
					RaisePropertyChanged();
				}
			}
		}

		private int _selectedYear { get; set; } = DateTime.Now.Year;
		public int SelectedYear
		{
			get => _selectedYear;
			set
			{
				if (value != _selectedYear && value >= _minYear && value <= _maxYear)
				{
					_selectedYear = value;
					RaisePropertyChanged();
					_daysInMonth[1] = IsLeapYear(_selectedYear) ? 29 : 28;
					SelectedMonth = SelectedMonth;
				}
			}
		}

		private static bool IsLeapYear(int year)
		{
			return (year % 400 == 0) || (year % 100 != 0 && year % 4 == 0);
		}

		#endregion

		#region Months

		private List<string> _months { get; set; } = new List<string>
		{
			@"January",
			@"February",
			@"March",
			@"April",
			@"May",
			@"June",
			@"July",
			@"August",
			@"September",
			@"October",
			@"November",
			@"December",
		};
		public List<string> Months => _months;
		private int[] _daysInMonth { get; set; } = new[] { 31, IsLeapYear(DateTime.Now.Year) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
		public string Month => Months[SelectedMonth];

		private int _selectedMonth { get; set; } = DateTime.Now.Month;
		public int SelectedMonth
		{
			get => _selectedMonth;
			set
			{
				_selectedMonth = value;
				RaisePropertyChanged();
				MaxDay = _daysInMonth[_selectedMonth-1];
			}
		}

		#endregion

		#region Days

		private int _maxDay { get; set; }
		public int MaxDay
		{
			get => _maxDay;
			set
			{
				_maxDay = value;
				RaisePropertyChanged();
				var date = new DateTime(SelectedYear, SelectedMonth, 1);
				var shift = (int)date.DayOfWeek;
				var fdw = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
				if (fdw == DayOfWeek.Monday)
					--shift;
				if (shift < 0) shift += 7;
				var days = new int[_maxDay + shift];
				for (var i = 1; i <= days.Length; ++i)
					days[i-1] = i < shift ? 0 : i-shift;
				Days = days;
				if (SelectedDay > _maxDay)
					SelectedDay = _maxDay;
			}
		}

		private int[] _days { get; set; } = new[]
		{
			1, 2, 3, 4, 5, 6, 7,
			8, 9, 10, 11, 12, 13, 14,
			15, 16, 17, 18, 19, 20, 21,
			22, 23, 24, 25, 26, 27, 28,
			29, 30, 31
		};
		public int[] Days
		{
			get => _days;
			private set
			{
				if (value != _days)
				{
					_days = value;
					RaisePropertyChanged();
				}
			}
		}

		private int _selectedDay { get; set; } = DateTime.Now.Day;
		public int SelectedDay
		{
			get => _selectedDay;
			set
			{
				if (value != _selectedDay)
				{
					_selectedDay = value;
					RaisePropertyChanged();
				}
			}
		}

		#endregion
	}
}
