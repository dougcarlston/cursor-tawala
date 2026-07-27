# Draft Letter to Sergei Lilichenko
*For Doug Carlston's review and sending*

---

Subject: Tawala Project Archives — Request for PostgreSQL Schema and Back-End Code

Dear Sergei,

I hope you're well. I'm writing because I've decided to revive the Tawala platform, and I'm working to reassemble as much of the original project as possible before beginning reconstruction.

As you know, I owned the company and retain all rights to the code — so there are no issues on that front. I'm simply trying to recover what survives before we start rebuilding from scratch.

Given your role as back-end developer and database architect on Tawala, you're the person I most need to hear from regarding the server-side code and PostgreSQL database. The most valuable things you could send would be:

- **PostgreSQL database schema** — table definitions, column types, indexes, constraints, and foreign key relationships. Even a `pg_dump --schema-only` export, or individual `CREATE TABLE` scripts, would be enormously valuable
- **Stored procedures and functions** — any PL/pgSQL functions, triggers, or stored procedures you wrote for the Tawala database
- **SVN repository archives or dumps** — any snapshot of the server-side codebase you retained from Subversion version control
- **Back-end server code** — any ASP.NET, C#, or other server-side files handling data access, process execution, form submission, or the Tawala runtime engine
- **API or web service definitions** — any WSDL files, service contracts, or API documentation for how the Designer communicated with the server
- **Database migration scripts** — any scripts used to evolve the schema over time
- **Any other server-side files** from your Tawala work — configuration files, deployment scripts, or partial backups are all useful

If you have an old laptop, hard drive, or backup disk from that era, it would genuinely be worth a look. Even partial or older versions give me a foundation to work from.

Please send whatever you find to [your email address], or let me know if a different transfer method would work better for larger files.

Your work on the database and back-end was what made the whole platform run, and recovering even a partial schema would save months of reconstruction effort.

Warm regards,
Doug Carlston
