using prj_BIZ_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prj_BIZ_System.Services
{
    public class NewsService : BaseService
    {
        public NewsService()
        {

        }

        /*select全部*/
        public IList<NewsModel> GetNewsAll()
        {
            return mapper.QueryForList<NewsModel>("News.SelectAll", null);
        }

        /*select單筆*/
        public NewsModel GetNewsOne(int news_no)
        {
            NewsModel newsModel = new NewsModel();
            newsModel.news_no = news_no;
            return (NewsModel)mapper.QueryForObject("News.SelectOne", newsModel);
        }

        /* insert方法*/
        public void InsertOne(NewsModel newsModel)
        {
            mapper.Insert("News.InsertOne", newsModel);
        }

        /*update方法*/
        public void UpdateOne(NewsModel newsModel)
        {
            mapper.Update("News.UpdateOne", newsModel);
        }
    }
}