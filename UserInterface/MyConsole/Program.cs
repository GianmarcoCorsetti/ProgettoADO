using DataConnection.ADO.Repositories;
using System;
using System.Collections.Generic;
using Model.Entities;
using System.Data.SqlClient;
using System.Data;
using Model.Exceptions;

namespace UserInterface.MyConsole {
    class Program {
        const string CONNECTION_STRING = @"
            Server = localhost;
            User = sa;
            Password = 1Secure*Password;
            Database = gestione_corsi
         ";
        const string SELECT_ALL_CLASSROOM = "SELECT id, nome, capacita_max, fisica_virtuale, computerizzata, proiettore FROM dbo.aule";
        const string SELECT_ID = @"SELECT id, nome, capacita_max, fisica_virtuale, computerizzata, proiettore FROM dbo.aule WHERE id = @id";
        const string UPDATE_ID = @"UPDATE dbo.aule 
                                SET nome = @newNome , 
                                    capacita_max = @capMax, 
                                    fisica_virtuale = @fisVirt,
                                    computerizzata = @comp, 
                                    proiettore = @proiettore
                                WHERE id = @idClasse ";
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //ClassroomRepository clrr = new ClassroomRepository();
            //Classroom clr = new Classroom();

            

            //Classroom c = FindClassroomById(2);
            //Console.WriteLine(c.Name);

            Classroom clNuova = new Classroom(2, "Aula bella", 150, true, false, false);
            Classroom clVecchia = UpdateClassroom(clNuova);
            Console.WriteLine(clVecchia.Name);
            List<Classroom> res = GetAllClassrooms();
            foreach (var cl in res)
            {
                Console.WriteLine(cl.Name);
            }

        }

        static List<Classroom> GetAllClassrooms()
        {
            SqlConnection conn = null;
            SqlDataReader reader = null ;
            List<Classroom> cls = new List<Classroom>();
            try
            {
                conn = new SqlConnection(CONNECTION_STRING);
                conn.Open();
                SqlCommand cmd = new SqlCommand(SELECT_ALL_CLASSROOM, conn);
                reader = cmd.ExecuteReader();
                while ( reader.Read() )
                {
                    long id = (long) reader.GetInt32(reader.GetOrdinal("id"));
                    //string nome = reader.GetString(reader.GetOrdinal("nome")); --> Guarda riga 45 !
                    int capMax = reader.GetInt32(reader.GetOrdinal("capacita_max"));
                    bool isFisica = reader.GetBoolean(reader.GetOrdinal("fisica_virtuale"));
                    bool isComputerizzata = reader.GetBoolean(reader.GetOrdinal("computerizzata"));
                    bool hasProiettore = reader.GetBoolean(reader.GetOrdinal("proiettore"));
                    Classroom cl = new Classroom(
                        newId: id,
                        newName: reader.GetString(reader.GetOrdinal("nome")), // nome
                        newMaxCapacity: capMax, 
                        isPhysical: isFisica, 
                        isComputerized: isComputerizzata, 
                        hasProjector: hasProiettore
                    );
                    cls.Add(cl);
                }
                return cls;
            }
            catch (SqlException e)
            {
                Console.WriteLine("Errore : " + e.Message);
                return null;
            }
            finally
            {
                if(reader != null)
                {// ci servirebbe un altro try .. catch
                    reader.Close();
                }
                if(conn != null)
                {// ci servirebbe un altro try .. catch
                    conn.Close();
                }
            }
        }
        static Classroom FindClassroomById (long idSearch)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    SqlCommand cmd = new SqlCommand(SELECT_ID, conn);
                    cmd.Parameters.Add("@id", SqlDbType.Int);
                    conn.Open();
                    cmd.Parameters["@id"].Value = idSearch;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // a volte non so se una var ha valore o meno, se è null può dare problemi ! 
                            bool? hasProiettore = reader.IsDBNull("proiettore") ? null : reader.GetBoolean(reader.GetOrdinal("proiettore"));
                            Classroom cl = new Classroom(
                                newId: idSearch,
                                newName: reader.GetString(reader.GetOrdinal("nome")),
                                newMaxCapacity: reader.GetInt32(reader.GetOrdinal("capacita_max")),
                                isPhysical: reader.GetBoolean(reader.GetOrdinal("fisica_virtuale")),
                                isComputerized: reader.GetBoolean(reader.GetOrdinal("computerizzata")),
                                //hasProjector: hasProiettore.HasValue ? hasProiettore.Value : false
                                hasProjector: hasProiettore
                                );
                            return cl;
                        }
                        return null;
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Errore : " + e.Message);
                return null;
            }
        }
        static Classroom UpdateClassroom ( Classroom c )
        {
            // Data una classroom con id esistente, se la classroom non esiste lancia una exception, la dobbiamo creare noi
            // Se trova la classroom, modifico le proprietà della classroom con quelle della classroom passata per parametro
            // ritornando la versione precedente dell'oggetto classroom. ( GL & HF )
            Classroom clPartenza = FindClassroomById(c.Id);

            if ( clPartenza == null )
            {
                throw new EntityNotFound("La classe con id {0} non esiste", c.Id);
            }

            try
            {
                using(SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    SqlCommand cmd = new SqlCommand(UPDATE_ID, conn);
                    cmd.Parameters.AddWithValue("@idClasse", c.Id); // metodo alternativo più veloce !!
                    //cmd.Parameters.Add("@idClasse", SqlDbType.Int);
                    cmd.Parameters.Add("@capMax", SqlDbType.Int);
                    cmd.Parameters.Add("@newNome", SqlDbType.NVarChar, 50);
                    cmd.Parameters.Add("@fisVirt", SqlDbType.Bit);
                    cmd.Parameters.Add("@comp", SqlDbType.Bit);
                    cmd.Parameters.Add("@proiettore", SqlDbType.Bit);
                    conn.Open();
                    //cmd.Parameters["@idClasse"].Value = c.Id;
                    cmd.Parameters["@capMax"].Value = c.MaxCapacity;
                    cmd.Parameters["@newNome"].Value = c.Name;
                    cmd.Parameters["@fisVirt"].Value = c.IsPhysical;
                    cmd.Parameters["@comp"].Value = c.IsComputerized;
                    cmd.Parameters["@proiettore"].Value = c.HasProjector;
                    cmd.ExecuteNonQuery();
                    return clPartenza;
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("Errore : " + e.Message);
                return null;
            }
        }
    }
}
