﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;

namespace WebApi.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class Categories: ControllerBase
    {
        private readonly ProductContext _context;

        public Categories(ProductContext context)
        {
            _context = context;
        }
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetWithProducts(int id)
        {
            var data = await _context.Categories.Include(x => x.Products).SingleOrDefaultAsync(x=>x.Id==id);
            if (data == null) { return NotFound(id); }
            return Ok(data);
        }
    }
}
