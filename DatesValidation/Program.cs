using System;
using System.Collections.Generic;
using System.Linq;

namespace DatesValidation
{
    class Program
    {
        public class MockingRoundDates
        {
            public List<DateTime> roundsDays { set; get; }
            public List<DateTime> roundsVacationDays { set; get; }

            public List<int> daysDiffs { get; set; }
            public MockingRoundDates()
            {
                daysDiffs = new List<int>();
                roundsDays = new List<DateTime>();
                roundsDays.Add(new DateTime(2020, 03, 25));
                roundsDays.Add(new DateTime(2020, 03, 29));
                roundsDays.Add(new DateTime(2020, 04, 01));
                roundsDays.Add(new DateTime(2020, 04, 05));
                roundsDays.Add(new DateTime(2020, 04, 08));
                roundsDays.Add(new DateTime(2020, 04, 12));
                roundsDays.Add(new DateTime(2020, 04, 15));
                roundsDays.Add(new DateTime(2020, 04, 19));
                roundsDays.Add(new DateTime(2020, 04, 22));
                roundsDays.Add(new DateTime(2020, 04, 26));
                getDaysDiffs();
            }
            private void getDaysDiffs()
            {
                //mentains gaps of days between each lecture
                for (int i = 0; i <= roundsDays.Count() - 2; i++)
                {
                    daysDiffs.Add((int)(roundsDays[i + 1] - roundsDays[i]).TotalDays);
                }
            }
        }

        public class MockingYearVacations
        {
            public string Name { get; set; }
            public DateTime startDate { get; set; }
            public DateTime endDate { get; set; }
        }

        public static MockingRoundDates validateDates(MockingRoundDates round, List<MockingYearVacations> vacations)
        {
            MockingRoundDates modifiedRound = round;
            if (!(round.roundsDays.Any(d => (vacations.Any(v => (v.startDate == d && v.endDate == d) || (v.startDate <= d && v.endDate >= d))))))
            {
                return modifiedRound;
            }

            if (vacations.Any(v => (v.startDate <= round.roundsDays[0] && v.endDate >= round.roundsDays[0])))
            {
                modifiedRound = reallocateRoundDaysIfAnyIsOnVacation(round, vacations, true);
            }
            else
            {
                modifiedRound = reallocateRoundDaysIfAnyIsOnVacation(round, vacations);

            }

            return validateDates(modifiedRound,vacations);
        }

        public static MockingRoundDates reallocateRoundDaysIfAnyIsOnVacation(MockingRoundDates round,
            List<MockingYearVacations> vacations,
            bool isFirstDayOfRoundOnVacation = false)
        {
            //this only if the first day in already a Vacation
            if (isFirstDayOfRoundOnVacation)
            {
                //reallocate first lecture to the second week,

                round.roundsDays[0] = round.roundsDays[0].AddDays(7);

                //Shift all other lectures by the right gaps.
                for (int i = 1; i < round.roundsDays.Count() - 1; i++)
                {
                    if (i != (round.roundsDays.Count() - 1))
                        round.roundsDays[i] = round.roundsDays[i - 1].AddDays(round.daysDiffs[i - 1]);
                }

                //Adjust the last lecture date
                round.roundsDays[round.roundsDays.Count() - 1] = round.roundsDays[(round.roundsDays.Count() - 1) - 1].AddDays(round.daysDiffs[round.daysDiffs.Count() - 1]);

                //check if round contains any vanacy day
                return addDaysInsteadOfVacationDays(round, round.daysDiffs, vacations);
            }

            //check if round contains any vanacy day
            return addDaysInsteadOfVacationDays(round, round.daysDiffs, vacations);
        }




        public static MockingRoundDates addDaysInsteadOfVacationDays(MockingRoundDates round,
            List<int> daysDiffs,
            List<MockingYearVacations> vacations
            )
        {
            var result = round.roundsDays.Where(d => (vacations.Any(v => (v.startDate == d && v.endDate == d) || (v.startDate <= d && v.endDate >= d)))).ToList();
            round.roundsVacationDays = result;
            var numberOfVacationDays = result.ToList().Count();
            int offsetOfInitialNumberOfDaysToIncrementWith = (daysDiffs.IndexOf(daysDiffs.Find(x => x == daysDiffs.Last()))) + 1;
            int currentDaysDiffOffset = 0;
            if (numberOfVacationDays != 0)
            {
                for (int i = 0; i < numberOfVacationDays; i++)
                {
                    if (currentDaysDiffOffset <= daysDiffs.Count() - 1)
                    {
                        currentDaysDiffOffset = offsetOfInitialNumberOfDaysToIncrementWith + i;
                        round.roundsDays.Add(round.roundsDays[round.roundsDays.Count() - 1].AddDays(daysDiffs[currentDaysDiffOffset]));
                    }
                    else if (currentDaysDiffOffset >= daysDiffs.Count() - 1)
                    {
                        currentDaysDiffOffset = 0;
                    }
                }
            }
            round.roundsDays = round.roundsDays.Except(round.roundsVacationDays).ToList();
            return round;
        }


        static void Main(string[] args)
        {
            List<MockingYearVacations> vacations = new List<MockingYearVacations>();

            vacations.Add(new MockingYearVacations
            {
                Name = "vacation_1",
                startDate = new DateTime(2020, 03, 23),
                endDate = new DateTime(2020, 03, 28)
            });
            vacations.Add(new MockingYearVacations
            {
                Name = "vacation_2",
                startDate = new DateTime(2020, 04, 08),
                endDate = new DateTime(2020, 04, 08)
            });
            vacations.Add(new MockingYearVacations
            {
                Name = "vacation_3",
                startDate = new DateTime(2020, 04, 19),
                endDate = new DateTime(2020, 04, 19)
            });
            vacations.Add(new MockingYearVacations
            {
                Name = "vacation_5",
                startDate = new DateTime(2020, 04, 22),
                endDate = new DateTime(2020, 05, 03)
            });
            vacations.Add(new MockingYearVacations
            {
                Name = "vacation_6",
                startDate = new DateTime(2020, 11, 03),
                endDate = new DateTime(2020, 11, 03)
            });
            vacations.Add(new MockingYearVacations
            {
                Name = "vacation_7",
                startDate = new DateTime(2020, 05, 13),
                endDate = new DateTime(2020, 05, 13)
            });
            vacations.Add(new MockingYearVacations
            {
                Name = "vacation_8",
                startDate = new DateTime(2020, 05, 17),
                endDate = new DateTime(2020, 05, 17)
            });
            vacations.Add(new MockingYearVacations
            {
                Name = "vacation_9",
                startDate = new DateTime(2020, 05, 20),
                endDate = new DateTime(2020, 05, 20)
            });
            vacations.Add(new MockingYearVacations
            {
                Name = "vacation_10",
                startDate = new DateTime(2020, 05, 24),
                endDate = new DateTime(2020, 05, 24)
            });

            var x = validateDates(new MockingRoundDates(), vacations);
            Console.ReadKey();
        }
    }
}
