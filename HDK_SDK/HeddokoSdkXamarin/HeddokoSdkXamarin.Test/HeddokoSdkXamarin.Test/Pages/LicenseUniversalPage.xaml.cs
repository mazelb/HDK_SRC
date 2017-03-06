/**
 * @file LicenseUniversalPage.xaml.cs
 * @brief Functionalities required to operate it.
 * @author Sergey Slepokurov (sergey@heddoko.com)
 * @date 11 2016
 * Copyright Heddoko(TM) 2017,  all rights reserved
*/
using System;
using System.Collections.Generic;
using System.Text;
using HeddokoSdkXamarin.Models;
using HeddokoSdkXamarin.Models.Requests;

namespace HeddokoSdkXamarin.Test.Pages
{
    public partial class LicenseUniversalPage
    {
        private readonly HeddokoClient _client;

        public LicenseUniversalPage(HeddokoClient client)
        {
            InitializeComponent();

            _client = client;
        }

        private async void TestOrganizations(object sender, EventArgs e)
        {
            try
            {
                ListCollection<Organization> organizations = await _client.GetAllOrganizations(new ListRequest { Take = 20, Skip = 0 });

                await ShowCollection(organizations.Collection, x => $"{x.Name}", "Organizations");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestTeams(object sender, EventArgs e)
        {
            try
            {
                var request = new TeamListRequest
                {
                    Take = 20,
                    Skip = 0,
                    OrganizationId = 1
                };

                ListCollection<Team> teams = await _client.GetAllTeams(request);

                await ShowCollection(teams.Collection, x => $"{x.Name}", "Teams");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestRecords(object sender, EventArgs e)
        {
            try
            {
                var request = new RecordListRequest
                {
                    Take = 20,
                    Skip = 0,
                    TeamId = 1
                };

                ListCollection<Record> records = await _client.GetAllRecords(request);

                await ShowCollection(records.Collection, x =>
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Empty);

                    foreach (Asset asset in x.Assets)
                    {
                        sb.AppendLine($"   {asset.Name}");
                    }

                    return $"user: {x.User?.Name}, assests:{sb.ToString()}";
                }, "Records");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void TestOrganizasionsErgoScores(object sender, EventArgs e)
        {
            try
            {
                ListCollection<Organization> organizations = await _client.GetAllOrganizations(new ListRequest { Take = 5, Skip = 0 });
                if (organizations.TotalCount == 0)
                {
                    await DisplayAlert("No organizations", "There are no organizations", "Ok");
                }
                else
                {
                    await ShowCollectionAsyncDisplay(
                        organizations.Collection,
                        async x =>
                        {
                            var sb = new StringBuilder($"Ergo Scores for organization {x.Name}:");

                            List<ErgoScore> ergoScores = await _client.GetOrganizationErgoScores(x.ID);
                            if (ergoScores.Count > 0)
                            {
                                foreach (ErgoScore ergoScore in ergoScores)
                                {
                                    sb.AppendLine($"    User id:{ergoScore.Id}, score:{ergoScore.Score}");
                                }
                            }
                            else
                            {
                                sb.AppendLine("    None");
                            }

                            return sb.ToString();
                        },
                        "Organizations Ergo Score");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }
}
