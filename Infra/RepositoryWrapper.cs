using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace JewelryStore.Infra
{
	public interface IRepositoryBase<T> where T : class
	{
		IEnumerable<T> Get(Expression<Func<T, bool>> expression = null);
		bool Any(Expression<Func<T, bool>> expression);
		T Add(T entity);
		void Add(List<T> entity);
		void Update(T entity);
		void Delete(T entity);
	}


	public class RepositoryWrapper<T>() : IRepositoryBase<T> where T : class
	{
		// All Replace with ADO.NET Logic
		public IEnumerable<T> Get(Expression<Func<T, bool>> expression = null) { /*Get Procedure with Condition*/ return null; }
		public bool Any(Expression<Func<T, bool>> expression) { /*Get Procedure with Condition and return true/false*/ return false; }
		public T Add(T entity) { return entity; } // Insert Procedure and return the inserted entity
		public void Add(List<T> entity) { /*Insert Procedure*/ }
		public void Update(T entity) { /*Update Procedure*/ }
		public void Delete(T entity) { /*Soft-Delete Procedure*/ }
	}

}
