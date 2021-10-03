namespace KillersLibraryTestBot.Services.Family {
    public class FamiliesService {
        public string MakeFamily(ulong value) {
            /*DataTable dataTable = await _dbConnection.GetAllAsync("member");
            DataTable dataTableFamily = await _dbConnection.GetAllAsync("family");
            DataTable dataTableFamilyConnections = await _dbConnection.GetAllByAsync("familyconnection", "familyID", value);
            DataTable dataTableMemberConnections = await _dbConnection.GetAllAsync("memberconnection");
            DataTable dataTableConnectionType = await _dbConnection.GetAllAsync("connectiontype");*/






            string json = value.ToString();






            return json;
        }

        public void FindOldestMember() {

        }
    }
}
