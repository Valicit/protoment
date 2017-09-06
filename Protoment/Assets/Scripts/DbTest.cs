using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.SqliteClient;

public class DbTest : MonoBehaviour {

	private string _constr = "";
	private IDbConnection _dbc;
	private IDbCommand _dbcm;
	private IDataReader _dbr;

	// Use this for initialization
	void Start () {
		_constr = "URI=file:" + Application.dataPath + "/Database/ProtomentDB.db";
		_dbc=new SqliteConnection(_constr);
		_dbc.Open();
		_dbcm=_dbc.CreateCommand();
		_dbcm.CommandText="SELECT `Job` FROM `Unit` WHERE `id`='"+1+"'";
		_dbr=_dbcm.ExecuteReader();

		while( _dbr.Read()){
			string job = _dbr.GetString (0);
			Debug.Log (job);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
