using SampleMVCTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SampleMVCTest.Controllers
{
    public class ItemsApiController : ApiController
    {
        private ItemDbContext _context;//=new ItemDbContext();

        public ItemsApiController()
        {
                _context = new ItemDbContext();
        }

        [HttpGet]
        public IHttpActionResult GetItems()
        {

            var items = _context.Items.ToList();
            //if (items == null || !items.Any())
            //{
            //    return NotFound();  // Return 404 if no items are found.
            //}
            return Ok(items);
        }

        [HttpGet]
        public IHttpActionResult GetItem(int id)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public IHttpActionResult AddItem(Item newItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _context.Items.Add(newItem);
            _context.SaveChanges();

            return Ok(newItem);

            //newItem.Id = items.Count > 0 ? items.Max(i => i.Id) + 1 : 1;
            //items.Add(newItem);
            //return Ok(newItem);
        }

        // PUT: api/ItemsApi/{id}
        [HttpPut]
        public IHttpActionResult UpdateItem(int id, Item updatedItem)
        {
            var itemInDb = _context.Items.Find(id);
            if (itemInDb == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            itemInDb.Name = updatedItem.Name;
            itemInDb.Description = updatedItem.Description;
            _context.SaveChanges();

            return Ok(itemInDb);
        }

        // DELETE: api/ItemsApi/{id}
        [HttpDelete]
        public IHttpActionResult DeleteItem(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null)
                return NotFound();

            _context.Items.Remove(item);
            _context.SaveChanges();

            return Ok();
        }
    }
}
