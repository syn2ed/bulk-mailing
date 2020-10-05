﻿using MailingProject.Controller;
using MailingProject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailingProject.View
{
    public partial class CampaignManagement : Form
    {
        public CampaignManagement()
        {
            InitializeComponent();
            this.Text = "Mailing APP - Campaign Management";
            //this.listView1.Items.
        }

        private void CampaignManagement_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        /* Au click du bouton "ajouter" pour les campagnes, on crée la campagne sur la view, en db puis on notifie l'user */
        private void button1_Click(object sender, EventArgs e)
        {
            /*
            if(textBox1.TextLength > 0)
            {
                this.addCampaign(textBox1.Text);
            }
            */

            Campaign newCampaign = new Campaign(textBox1.Text); //création de l'objet Campagne
            MainController.getInstance().addCampaign(newCampaign); //ajout de la campagne à la db
            MainController.getInstance().UpdateCampaignListFromDb(); //Mise à jour de la liste sur la view depuis la db suite à l'ajout de la nouvelle campagne


            System.Windows.Forms.MessageBox.Show("Nouvelle campagne ajoutée !");
        }

        private void addCampaign(string campaignName)
        {
            //MainController.getInstance().AddCampaign(new Campaign(textBox1.Text));
            listView1.Items.Add(new ListViewItem(campaignName));
            textBox1.Clear();
        }

        /**
         * Mise à jour de la liste des campagnes en view depuis celles passés en param
         */

        public void UpdateCampaignListFromDb(List<Campaign> campaigns)
        {
            listView1.Clear();

            foreach(Campaign campaign in campaigns)
            {
                ListViewItem campaignViewTime = new ListViewItem();
                ListViewItem.ListViewSubItem campaignIdSubItem = new ListViewItem.ListViewSubItem();

                campaignViewTime.Text = campaign.name;
                campaignIdSubItem.Name = "CampaignId";
                campaignIdSubItem.Text = campaign.campaignId.ToString();

                campaignViewTime.SubItems.Add(campaignIdSubItem);
                listView1.Items.Add(campaignViewTime);
            }
        }

        /**
         * Mise à jour de la liste des fichires d'emails en view depuis celles passés en param
         */
        private void UpdateEmailsFileListFromDb(ICollection<EmailsFile> eEmailsFiles)
        {
            listView2.Clear();

            foreach (EmailsFile emailsFile in eEmailsFiles)
            {
                ListViewItem campaignViewTime = new ListViewItem();
                ListViewItem.ListViewSubItem campaignIdSubItem = new ListViewItem.ListViewSubItem();

                campaignViewTime.Text = emailsFile.path;
                campaignIdSubItem.Name = "emailsFileId";
                campaignIdSubItem.Text = emailsFile.emailsFileId.ToString();

                campaignViewTime.SubItems.Add(campaignIdSubItem);
                listView2.Items.Add(campaignViewTime);
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
        /* Au click d'une campagne, on met à jour la partie mail listing */
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)// Si un élement est bien selectionné (et que c'est pas un click à blanc)
            {
                groupBox2.Visible = true; //Affichage de la partie emails liés à la campagne selectionnée

                /* clear de la partie listing mails */
                this.listView2.Clear();
                this.listView3.Clear();

                this.showCampaignInformationsFromDb(); //Récupération et affichage de la liste des fichiers d'emails liés à cette campagne
            } else
            {
                groupBox2.Visible = false;
            }
        }

        /*
         * Récupération et affichage des paths des fichiers d'emails associés à la campagne selectionnée
         */
        private void showCampaignInformationsFromDb()
        {
            // Récupération des paths des fichiers d'emails liés à la campagne selectionnée sur cette view
            ICollection<EmailsFile> emailsFiles = MainController.getInstance().getCampaignEmailsFilesById(Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text));

            /*
             * On parcours les fichiers d'emails récupérés en DB
             * Pour chacun, on l'affiche sur la listView2 (qui correspond à la liste des fichiers associés à la campagne selectionnée)
             * Et pour garder l'id de cet EmailsFile, on l'insère en subitem au cas où on en a besoin
             */
            foreach(EmailsFile emailFile in emailsFiles)
            {
                ListViewItem emailFileListViewItem = new ListViewItem(emailFile.path);
                ListViewItem.ListViewSubItem emailFileListViewSubItem = new ListViewItem.ListViewSubItem();

                emailFileListViewSubItem.Name = "EmailsFileId";
                emailFileListViewSubItem.Text = emailFile.emailsFileId.ToString();

                emailFileListViewItem.SubItems.Add(emailFileListViewSubItem);

                listView2.Items.Add(emailFileListViewItem);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        /**
         * Au click du bouton "Ajouter fichier", on ouvre la fenêtre de sélection d'un fichier de mail
         * Puis on ajoute ce fichier sur la view et en DB
         */
        private void button2_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSV file (*.csv)|*.csv| Txt file (*.txt)|*.txt"; // file types, that will be allowed to upload
            dialog.Multiselect = true; // allow/deny user to upload more than one file at a time
            int campaignSelectedId = Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text); //campaignId de la campagne selectionnée

            if (dialog.ShowDialog() == DialogResult.OK) // if user clicked OK
            {
                String path = dialog.FileName; // get name of file
                using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open), new UTF8Encoding())) // do anything you want, e.g. read it
                {
                    EmailsFile newEmailsFile = new EmailsFile(path);

                    MainController.getInstance().AddEmailsFileByCampaignId(campaignSelectedId, newEmailsFile); //Ajout du nouveau EmailsFile associé à la campagne selectionnée
                    this.UpdateEmailsFileListFromDb(MainController.getInstance().getCampaignEmailsFilesById(campaignSelectedId)); //Mise à jour de la liste de fichiers d'emails
                    //listView2.Items.Add(new ListViewItem(dialog.FileName));
                    //File.Copy(path, "Model/EmailListFiles");
                }
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
